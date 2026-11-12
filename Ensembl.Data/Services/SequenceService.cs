using System.Data;
using System.Text;
using Ensembl.Data.Entities;
using Ensembl.Data.Helpers;
using Ensembl.Data.Models.Liftover;
using Microsoft.EntityFrameworkCore;

using GeneModel = Ensembl.Data.Models.Gene;
using ProteinModel = Ensembl.Data.Models.Protein;
using TranscriptEntity = Ensembl.Data.Entities.Transcript;
using TranscriptModel = Ensembl.Data.Models.Transcript;

namespace Ensembl.Data.Services;

public class SequenceService
{
    private readonly EnsemblDbContext _dbContext;
    private readonly Dictionary<string, int> _seqRegionIds = [];

    public readonly byte Genome;


    public SequenceService(EnsemblDbContext dbContext)
    {
        _dbContext = dbContext;

        Genome = dbContext.Genome;
    }


    public string GetForGene(GeneModel gene)
    {
        return ReadSequence(gene.Chromosome, gene.Start, gene.End, gene.Strand);
    }

    public string GetForTranscript(TranscriptModel model, SequenceType type)
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

    public string GetForRegion(string chromosome, int start, int end)
    {
        return ReadSequence(chromosome, start, end, true);
    }

    public Dictionary<int, string> GetForRegions(CoordinateRegion[] regions)
    {
        var result = new Dictionary<int, char[]>();
        if (regions.Length == 0)
            return [];

        var version = $"GRCh{_dbContext.Genome}";
        var chromosomeNames = regions.Select(region => region.Chromosome).Distinct().ToArray();
        var chromosomes = _dbContext.SeqRegions.AsNoTracking()
            .Where(region => chromosomeNames.Contains(region.Name))
            .Where(region => region.CoordSystem.Name == "chromosome" && region.CoordSystem.Version == version)
            .Select(region => new { region.Name, region.SeqRegionId })
            .ToDictionary(region => region.Name, region => region.SeqRegionId);
        var chromosomeIds = chromosomes.Values.ToArray();
        var exceptions = _dbContext.AssemblyExceptions.AsNoTracking()
            .Where(exception => chromosomeIds.Contains(exception.SeqRegionId))
            .Where(exception => exception.ExceptionType == "PAR")
            .ToArray();
        var directRegions = _dbContext.Dnas.AsNoTracking()
            .Where(dna => chromosomeIds.Contains(dna.SeqRegionId))
            .Select(dna => dna.SeqRegionId)
            .ToHashSet();
        var mappings = _dbContext.AssemblyMappings.AsNoTracking()
            .Where(mapping => chromosomeIds.Contains(mapping.AssemblySeqRegionId))
            .Where(mapping => mapping.ComponentSeqRegion.CoordSystem.Attrib.Contains("sequence_level"))
            .OrderBy(mapping => mapping.AssemblyStart)
            .ToArray()
            .GroupBy(mapping => mapping.AssemblySeqRegionId)
            .ToDictionary(group => group.Key, group => group.ToArray());
        var parts = new List<SequencePart>();

        foreach (var region in regions)
        {
            var seqRegionId = chromosomes[region.Chromosome];
            var sequence = new string('N', region.End - region.Start + 1).ToCharArray();
            result[region.Id] = sequence;

            if (exceptions.Any(exception =>
                    exception.SeqRegionId == seqRegionId &&
                    exception.SeqRegionStart <= region.End &&
                    exception.SeqRegionEnd >= region.Start))
            {
                result[region.Id] = ReadSequence(seqRegionId, region.Start, region.End).ToCharArray();
                continue;
            }

            if (directRegions.Contains(seqRegionId))
            {
                parts.Add(new SequencePart(parts.Count, region.Id, seqRegionId, region.Start,
                    region.End - region.Start + 1, 0, true));
                continue;
            }

            if (!mappings.TryGetValue(seqRegionId, out var chromosomeMappings))
                continue;

            foreach (var mapping in chromosomeMappings
                         .Where(mapping => mapping.AssemblyStart <= region.End && mapping.AssemblyEnd >= region.Start))
            {
                var mappingStart = Math.Max(region.Start, mapping.AssemblyStart);
                var mappingEnd = Math.Min(region.End, mapping.AssemblyEnd);
                var componentStart = mapping.Orientation == 1
                    ? mapping.ComponentStart + mappingStart - mapping.AssemblyStart
                    : mapping.ComponentEnd - mappingEnd + mapping.AssemblyStart;

                parts.Add(new SequencePart(
                    parts.Count,
                    region.Id,
                    mapping.ComponentSeqRegionId,
                    componentStart,
                    mappingEnd - mappingStart + 1,
                    mappingStart - region.Start,
                    mapping.Orientation == 1));
            }
        }

        foreach (var part in ReadParts(parts))
        {
            var sequence = part.Forward
                ? part.Sequence
                : NucleotideSequence.ReverseComplement(part.Sequence);
            sequence.CopyTo(0, result[part.RegionIndex], part.Offset, sequence.Length);
        }

        return result.ToDictionary(item => item.Key, item => new string(item.Value));
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
        if (!_seqRegionIds.TryGetValue(chromosome, out var seqRegionId))
        {
            seqRegionId = _dbContext.SeqRegions.AsNoTracking()
                .Where(region => region.Name == chromosome)
                .Where(region => region.CoordSystem.Name == "chromosome" && region.CoordSystem.Version == version)
                .Select(region => region.SeqRegionId)
                .Single();
            _seqRegionIds[chromosome] = seqRegionId;
        }

        return ReadSequence(seqRegionId, start, end, forward);
    }

    private string ReadSequence(int seqRegionId, int start, int end, bool forward)
    {
        var sequence = ReadSequence(seqRegionId, start, end);
        return forward ? sequence : NucleotideSequence.ReverseComplement(sequence);
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
            sequence.Append(exception.Orientation == 1 ? mapped : NucleotideSequence.ReverseComplement(mapped));
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
                component = NucleotideSequence.ReverseComplement(component);

            component.CopyTo(0, sequence, mappingStart - start, component.Length);
        }

        return new string(sequence);
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

    private IEnumerable<SequencePartResult> ReadParts(List<SequencePart> parts)
    {
        const int batchSize = 500;
        var connection = _dbContext.Database.GetDbConnection();
        var closeConnection = connection.State != ConnectionState.Open;

        if (closeConnection)
            connection.Open();

        try
        {
            foreach (var batch in parts.Chunk(batchSize))
            {
                using var command = connection.CreateCommand();
                command.CommandText = string.Join(" UNION ALL ", batch.Select(part =>
                    $"SELECT {part.Id}, UPPER(SUBSTRING(sequence, {part.Start}, {part.Length})) " +
                    $"FROM dna WHERE seq_region_id = {part.SeqRegionId}"));

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var part = parts[id];

                    yield return new SequencePartResult(
                        part.RegionIndex,
                        part.Offset,
                        part.Forward,
                        reader.GetString(1));
                }
            }
        }
        finally
        {
            if (closeConnection)
                connection.Close();
        }
    }


    private record SequenceEdit(int Start, int Length, string Replacement);

    private record SequencePart(
        int Id,
        int RegionIndex,
        int SeqRegionId,
        int Start,
        int Length,
        int Offset,
        bool Forward);

    private record SequencePartResult(int RegionIndex, int Offset, bool Forward, string Sequence);
}
