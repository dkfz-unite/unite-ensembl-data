using System.Linq.Expressions;
using Ensembl.Data.Entities.Constants;
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

        var entity = GetQuery().FirstOrDefault(e => e.StableId == id);

        if (entity == null)
        {
            return null;
        }

        var objectXref = GetObjectXrefQuery().FirstOrDefault(e => e.EnsemblId == entity.TranslationId);

        return Convert(entity, objectXref, expand);
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

        var entities = GetQuery().Where(e => ids.Contains(e.StableId)).ToArray();

        var entityIds = entities.Select(e => e.TranslationId).ToArray();

        var objectXrefs = GetObjectXrefQuery()
            .Where(e => entityIds.Contains(e.EnsemblId))
            .ToDictionary(e => e.EnsemblId);

        return entities.Select(entity => Convert(entity, objectXrefs[entity.TranslationId], expand)).ToArray();
    }


    /// <summary>
    /// Finds protein by symbol.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found protein.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein FindByName(string symbol, bool expand = false)
    {
        if (string.IsNullOrEmpty(symbol))
        {
            throw new ArgumentException("Protein symbol is missing.", nameof(symbol));
        }

        return FindViaXref(e => e.Xref.DisplayLabel == symbol, expand);
    }

    /// <summary>
    /// Finds proteins by their symbols.
    /// </summary>
    /// <param name="symbols">Symbols list</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found proteins.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein[] FindByName(IEnumerable<string> symbols, bool expand = false)
    {
        if (symbols == null)
        {
            throw new ArgumentException("Protein symbols are missing.", nameof(symbols));
        }

        return FindAllViaXref(e => symbols.Contains(e.Xref.DisplayLabel), expand);
    }


    /// <summary>
    /// Finds protein by accession.
    /// </summary>
    /// <param name="accession">Accession</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found protein.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein FindByAccession(string accession, bool expand = false)
    {
        if (string.IsNullOrEmpty(accession))
        {
            throw new ArgumentException("Protein accession is missing.", nameof(accession));
        }

        return FindViaXref(e => e.Xref.DbprimaryAcc == accession, expand);
    }

    /// <summary>
    /// Finds proteins by their accessions.
    /// </summary>
    /// <param name="accessions">Accessions list</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found proteins.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Protein[] FindByAccession(IEnumerable<string> accessions, bool expand = false)
    {
        if (accessions == null)
        {
            throw new ArgumentException("Protein accessions are missing.", nameof(accessions));
        }

        return FindAllViaXref(e => accessions.Contains(e.Xref.DbprimaryAcc), expand);
    }


    internal Protein Get(int id, bool expand = false)
    {
        var entity = GetQuery().FirstOrDefault(e => e.TranslationId == id);

        var objectXref = GetObjectXrefQuery().FirstOrDefault(e => e.EnsemblId == id);

        return Convert(entity, objectXref, expand);
    }


    private IQueryable<Entities.Translation> GetQuery()
    {
        return _dbContext.Translations
            .Include(e => e.Transcript)
            .Include(e => e.StartExon)
            .Include(e => e.EndExon);
    }

    private IQueryable<Entities.ObjectXref> GetObjectXrefQuery()
    {
        return _dbContext.ObjectXrefs
            .Include(e => e.Xref.ExternalDb)
            .OrderByDescending(e => e.Xref.ExternalDb.Priority)
            .Where(e => e.EnsemblObjectType == ObjectType.Protein);
    }

    private Protein FindViaXref(Expression<Func<Entities.ObjectXref, bool>> predicate, bool expand = false)
    {
        var objectXref = GetObjectXrefQuery().FirstOrDefault(predicate);

        if (objectXref == null)
        {
            return null;
        }

        var entity = GetQuery().FirstOrDefault(e => e.TranslationId == objectXref.EnsemblId);

        return Convert(entity, objectXref, expand);
    }

    private Protein[] FindAllViaXref(Expression<Func<Entities.ObjectXref, bool>> predicate, bool expand = false)
    {
        var objectXrefs = GetObjectXrefQuery()
            .Where(predicate)
            .ToDictionary(e => e.EnsemblId);

        var entities = GetQuery()
            .Where(e => objectXrefs.Keys.Contains(e.TranslationId))
            .ToArray();

        return entities.Select(entity => Convert(entity, objectXrefs[entity.TranslationId], expand)).ToArray();
    }

    private Protein Convert(Entities.Translation entity, Entities.ObjectXref objectXref, bool expand = false)
    {
        if (entity != null)
        {
            var protein = new Protein(entity, objectXref);

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
