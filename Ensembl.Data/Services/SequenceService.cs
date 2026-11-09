using System.Text;
using Ensembl.Data.Entities;
using Ensembl.Data.Helpers;
using Ensembl.Data.Services.Configuration.Constants;
using Microsoft.EntityFrameworkCore;
using GeneModel = Ensembl.Data.Models.Gene;
using ProteinModel = Ensembl.Data.Models.Protein;
using TranscriptEntity = Ensembl.Data.Entities.Transcript;
using TranscriptModel = Ensembl.Data.Models.Transcript;

namespace Ensembl.Data.Services;

public class SequenceService
{
    private readonly EnsemblDbContext _dbContext;


    public SequenceService(EnsemblDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public string GetForGene(GeneModel gene)
    {
        return ReadSequence(gene.Chromosome, gene.Start, gene.End, gene.Strand);
    }

    public string GetForTranscript(TranscriptModel model, string type)
    {
        if (type == SequenceType.Genomic)
            return ReadSequence(model.Chromosome, model.Start, model.End, model.Strand);

        var transcript = _dbContext.Transcripts.AsNoTracking()
            .Single(entity => entity.StableId == model.Id);

        Translation translation = null;
        if (type == SequenceType.Cds)
        {
            if (!transcript.CanonicalTranslationId.HasValue)
                return null;

            translation = _dbContext.Translations.AsNoTracking()
                .Single(entity => entity.TranslationId == transcript.CanonicalTranslationId.Value);
        }

        return ReadTranscript(transcript, translation);
    }

    public string GetForProtein(ProteinModel protein)
    {
        var translation = _dbContext.Translations.AsNoTracking()
            .Include(entity => entity.Transcript)
            .Single(entity => entity.StableId == protein.Id);
        var coding = ReadTranscript(translation.Transcript, translation);
        var codonTable = _dbContext.SeqRegionAttribs.AsNoTracking()
            .Where(attribute => attribute.SeqRegionId == translation.Transcript.SeqRegionId)
            .Where(attribute => attribute.AttribType.Code == "codon_table")
            .Select(attribute => attribute.Value)
            .FirstOrDefault();
        var peptide = PeptideSequence.Translate(coding, codonTable == "2");
        var attributes = _dbContext.TranslationAttribs.AsNoTracking()
            .Where(attribute => attribute.TranslationId == translation.TranslationId)
            .Where(attribute => attribute.AttribType.Code == "initial_met" ||
                                attribute.AttribType.Code == "_selenocysteine" ||
                                attribute.AttribType.Code == "amino_acid_sub" ||
                                attribute.AttribType.Code == "_stop_codon_rt")
            .Select(attribute => attribute.Value)
            .ToArray();

        return ApplyEdits(peptide, ReadEdits(attributes));
    }


    private string ReadTranscript(TranscriptEntity transcript, Translation translation)
    {
        var exons = _dbContext.ExonTranscripts.AsNoTracking()
            .Where(entity => entity.TranscriptId == transcript.TranscriptId)
            .OrderBy(entity => entity.Rank)
            .Select(entity => entity.Exon)
            .ToArray();

        var parts = exons.Select(exon => ReadSequence(
            exon.SeqRegionId,
            exon.SeqRegionStart,
            exon.SeqRegionEnd,
            exon.SeqRegionStrand == 1)).ToArray();

        var sequence = string.Concat(parts);

        var attributes = _dbContext.TranscriptAttribs.AsNoTracking()
            .Where(attribute => attribute.TranscriptId == transcript.TranscriptId)
            .Where(attribute => attribute.AttribType.Code == "_rna_edit" ||
                                attribute.AttribType.Code == "_transl_start" ||
                                attribute.AttribType.Code == "_transl_end")
            .Select(attribute => new { attribute.AttribType.Code, attribute.Value })
            .ToArray();

        var edits = ReadEdits(attributes
            .Where(attribute => attribute.Code == "_rna_edit")
            .Select(attribute => attribute.Value));

        sequence = ApplyEdits(sequence, edits);

        if (translation == null)
            return sequence;

        var startIndex = Array.FindIndex(exons, exon => exon.ExonId == translation.StartExonId);
        var endIndex = Array.FindIndex(exons, exon => exon.ExonId == translation.EndExonId);
        var start = parts.Take(startIndex).Sum(part => part.Length) + translation.SeqStart;
        var end = parts.Take(endIndex).Sum(part => part.Length) + translation.SeqEnd;

        foreach (var edit in edits)
        {
            var shift = edit.Replacement.Length - edit.Length;
            if (edit.Start < start)
                start += shift;
            if (edit.Start <= end + 1)
                end += shift;
        }

        var annotatedStart = attributes.FirstOrDefault(attribute => attribute.Code == "_transl_start")?.Value;
        if (annotatedStart != null)
            start = int.Parse(annotatedStart);

        var annotatedEnd = attributes.FirstOrDefault(attribute => attribute.Code == "_transl_end")?.Value;
        if (annotatedEnd != null)
            end = int.Parse(annotatedEnd);

        var phase = Math.Max(0, (int)exons[startIndex].Phase);
        return new string('N', phase) + sequence[(start - 1)..end];
    }

    private string ReadSequence(string chromosome, int start, int end, bool forward)
    {
        var version = $"GRCh{_dbContext.Genome}";
        var seqRegionId = _dbContext.SeqRegions.AsNoTracking()
            .Where(region => region.Name == chromosome)
            .Where(region => region.CoordSystem.Name == "chromosome" && region.CoordSystem.Version == version)
            .Select(region => region.SeqRegionId)
            .Single();

        return ReadSequence(seqRegionId, start, end, forward);
    }

    private string ReadSequence(int seqRegionId, int start, int end, bool forward)
    {
        var sequence = ReadSequence(seqRegionId, start, end);
        return forward ? sequence : ReverseComplement(sequence);
    }

    private string ReadSequence(int seqRegionId, int start, int end)
    {
        var exceptions = _dbContext.AssemblyExceptions
            .AsNoTracking()
            .Where(exception => exception.SeqRegionId == seqRegionId)
            .Where(exception => exception.ExceptionType == "PAR")
            .Where(exception => exception.SeqRegionStart <= end && exception.SeqRegionEnd >= start)
            .OrderBy(exception => exception.SeqRegionStart)
            .ToArray();

        if (exceptions.Length == 0)
            return ReadAssembly(seqRegionId, start, end);

        var sequence = new StringBuilder(end - start + 1);
        var cursor = start;

        foreach (var exception in exceptions)
        {
            var exceptionStart = Math.Max(start, exception.SeqRegionStart);
            var exceptionEnd = Math.Min(end, exception.SeqRegionEnd);
            if (cursor < exceptionStart)
                sequence.Append(ReadAssembly(seqRegionId, cursor, exceptionStart - 1));

            var mappedStart = exception.Orientation == 1
                ? exception.ExceptionSeqRegionStart + exceptionStart - exception.SeqRegionStart
                : exception.ExceptionSeqRegionEnd - exceptionEnd + exception.SeqRegionStart;
            var mappedEnd = mappedStart + exceptionEnd - exceptionStart;
            var mapped = ReadSequence(exception.ExceptionSeqRegionId, mappedStart, mappedEnd);
            sequence.Append(exception.Orientation == 1 ? mapped : ReverseComplement(mapped));
            cursor = exceptionEnd + 1;
        }

        if (cursor <= end)
            sequence.Append(ReadAssembly(seqRegionId, cursor, end));

        return sequence.ToString();
    }

    private string ReadAssembly(int seqRegionId, int start, int end)
    {
        var length = end - start + 1;

        var dna = _dbContext.Dnas.AsNoTracking()
            .Where(entity => entity.SeqRegionId == seqRegionId)
            .Select(entity => entity.Sequence.Substring(start - 1, length))
            .SingleOrDefault();

        if (dna != null)
            return dna.ToUpperInvariant();

        var mappings = _dbContext.AssemblyMappings.AsNoTracking()
            .Where(mapping => mapping.AssemblySeqRegionId == seqRegionId)
            .Where(mapping => mapping.AssemblyStart <= end && mapping.AssemblyEnd >= start)
            .Where(mapping => mapping.ComponentSeqRegion.CoordSystem.Attrib.Contains("sequence_level"))
            .OrderBy(mapping => mapping.AssemblyStart)
            .ToArray();

        var sequence = new string('N', length).ToCharArray();

        foreach (var mapping in mappings)
        {
            var mappingStart = Math.Max(start, mapping.AssemblyStart);
            var mappingEnd = Math.Min(end, mapping.AssemblyEnd);
            var componentStart = mapping.Orientation == 1
                ? mapping.ComponentStart + mappingStart - mapping.AssemblyStart
                : mapping.ComponentEnd - mappingEnd + mapping.AssemblyStart;
            var componentEnd = componentStart + mappingEnd - mappingStart;
            var component = ReadAssembly(mapping.ComponentSeqRegionId, componentStart, componentEnd);
            if (mapping.Orientation == -1)
                component = ReverseComplement(component);

            component.CopyTo(0, sequence, mappingStart - start, component.Length);
        }

        return new string(sequence);
    }

    private static string ReverseComplement(string sequence)
    {
        return string.Create(sequence.Length, sequence, (result, source) =>
        {
            for (var index = 0; index < result.Length; index++)
                result[index] = Complement(source[source.Length - index - 1]);
        });
    }

    private static char Complement(char nucleotide)
    {
        return nucleotide switch
        {
            'A' => 'T',
            'C' => 'G',
            'G' => 'C',
            'T' => 'A',
            'R' => 'Y', 'Y' => 'R', 'S' => 'S', 'W' => 'W',
            'K' => 'M', 'M' => 'K', 'B' => 'V', 'V' => 'B', 'D' => 'H', 'H' => 'D',
            _ => 'N'
        };
    }

    private static SequenceEdit[] ReadEdits(IEnumerable<string> values)
    {
        return values.Select(value =>
        {
            var parts = value.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            var start = int.Parse(parts[0]);
            var end = int.Parse(parts[1]);
            var replacement = parts.Length == 3 ? parts[2].Trim().ToUpperInvariant() : "";
            return new SequenceEdit(start, end - start + 1, replacement);
        }).OrderByDescending(edit => edit.Start).ToArray();
    }

    private static string ApplyEdits(string sequence, SequenceEdit[] edits)
    {
        foreach (var edit in edits)
        {
            var offset = edit.Start - 1;
            sequence = sequence.Remove(offset, edit.Length).Insert(offset, edit.Replacement);
        }

        return sequence;
    }


    private record SequenceEdit(int Start, int Length, string Replacement);
}
