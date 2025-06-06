using Ensembl.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services;

public class ProteinSearchService
{
    private readonly EnsemblDbContext _dbContext;


    public ProteinSearchService(EnsemblDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// Finds protein by Ensembl stable identifier.
    /// </summary>
    /// <param name="id">Stable identifier</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found protein.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein Find(string id, bool expand = false)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Protein id is missing.", nameof(id));
        }

        var entity = GetQuery().FirstOrDefault(entity => entity.StableId == id);

        return Convert(entity, expand);
    }

    /// <summary>
    /// Finds proteins by their stable identifiers.
    /// </summary>
    /// <param name="ids">Stable identifiers list</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found proteins.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein[] Find(IEnumerable<string> ids, bool expand = false)
    {
        if (ids == null)
        {
            throw new ArgumentException("Protein ids are missing", nameof(ids));
        }

        var entities = GetQuery().Where(entity => ids.Contains(entity.StableId)).ToArray();

        return entities.Select(entity => Convert(entity, expand)).ToArray();
    }

    internal Protein Get(int id, bool expand = false)
    {
        var entity = GetQuery().FirstOrDefault(e => e.TranslationId == id);

        return Convert(entity, expand);
    }


    private IQueryable<Entities.Translation> GetQuery()
    {
        return _dbContext.Translations
            .Include(e => e.Transcript)
            .Include(e => e.StartExon)
            .Include(e => e.EndExon);
    }

    private Protein Convert(Entities.Translation entity, bool expand = false)
    {
        if (entity != null)
        {
            var protein = new Protein(entity);

            protein.Length = GetProteinLength(entity, protein.Start, protein.End);

            if (expand)
            {
                protein.Features = GetProteinFeatures(entity);
            }

            return protein;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Retrieves number of amino acids in a protein.
    /// </summary>
    /// <param name="entity">Translation</param>
    /// <param name="start">Genomic start of the translation</param>
    /// <param name="end">Genomic end of the translation</param>
    /// <returns>Protein length in amino acids.</returns>
    private int GetProteinLength(Entities.Translation entity, int start, int end)
    {
        var length = 0;

        var exons = _dbContext.ExonTranscripts
                .Include(e => e.Exon)
                .Where(e => e.TranscriptId == entity.TranscriptId)
                .Where(e => e.Exon.SeqRegionStart > start)
                .Where(e => e.Exon.SeqRegionEnd < end)
                .Select(e => e.Exon);

        if (entity.StartExon.SeqRegionStrand == 1)
        {
            length += entity.StartExon.SeqRegionEnd - start;
            length += end - entity.EndExon.SeqRegionStart;
            length += exons.Sum(exon => exon.SeqRegionEnd - exon.SeqRegionStart + 1);
        }
        else
        {
            length += end - entity.StartExon.SeqRegionStart;
            length += entity.EndExon.SeqRegionEnd - start;
            length += exons.Sum(exon => exon.SeqRegionEnd - exon.SeqRegionStart + 1);
        }

        return length / 3;
    }

    /// <summary>
    /// Retrieves pfam features of a protein.
    /// </summary>
    /// <param name="entity">Translation</param>
    /// <returns>Array of pfam protein features.</returns>
    private ProteinFeature[] GetProteinFeatures(Entities.Translation entity)
    {
        return _dbContext.ProteinFeatures
            .Where(e => e.TranslationId == entity.TranslationId)
            .Where(e => e.HitName.StartsWith("PF"))
            .Select(e => new ProteinFeature(e))
            .ToArray();
    }
}
