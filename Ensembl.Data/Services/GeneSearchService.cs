using Ensembl.Data.Models;
using Ensembl.Data.Services.Configuration.Constants;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services;

public class GeneSearchService
{
    private readonly EnsemblDbContext _dbContext;
    private readonly TranscriptSearchService _transcriptSearchService;
    private readonly ProteinSearchService _proteinSearchService;


    public GeneSearchService(EnsemblDbContext dbContext)
    {
        _dbContext = dbContext;
        _transcriptSearchService = new TranscriptSearchService(_dbContext);
        _proteinSearchService = new ProteinSearchService(_dbContext);
    }

    /// <summary>
    /// Finds gene by Ensembl stable identifier.
    /// </summary>
    /// <param name="id">Stable identifier</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found gene.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Gene Find(string id, bool length = false, bool expand = false)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Gene id is missing.", nameof(id));
        }

        var entity = GetQuery().FirstOrDefault(entity => entity.StableId == id);

        return Convert(entity, length, expand);
    }

    /// <summary>
    /// Finds genes by their Ensembl stable identifiers.
    /// </summary>
    /// <param name="ids">Stable identifiers list</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found genes.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Gene[] Find(IEnumerable<string> ids, bool length = false, bool expand = false)
    {
        if (ids == null)
        {
            throw new ArgumentException("Gene ids are missing.", nameof(ids));
        }

        var entities = GetQuery().Where(entity => ids.Contains(entity.StableId)).ToArray();

        return entities.Select(entity => Convert(entity, length, expand)).ToArray();
    }

    /// <summary>
    /// Finds gene by symbol.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found gene.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Gene FindByName(string symbol, bool length = false, bool expand = false)
    {
        if (string.IsNullOrEmpty(symbol))
        {
            throw new ArgumentException("Gene symbol is missing.", nameof(symbol));
        }

        var entity = GetQuery().FirstOrDefault(entity => entity.Xref.DisplayLabel == symbol);

        return Convert(entity, length, expand);
    }

    /// <summary>
    /// Finds genes by their symbols.
    /// </summary>
    /// <param name="symbols">Symbols</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found genes.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Gene[] FindByName(IEnumerable<string> symbols, bool length = false, bool expand = false)
    {
        if (symbols == null)
        {
            throw new ArgumentException("Gene symbols are missing.", nameof(symbols));
        }

        var entities = GetQuery().Where(entity => symbols.Contains(entity.Xref.DisplayLabel)).ToArray();

        return entities.Select(entity => Convert(entity, length, expand)).ToArray();
    }


    private IQueryable<Entities.Gene> GetQuery()
    {
        var coordSystemIds = _dbContext.GetCoordSystemIds();
        var chromosomeNames = GenomeSettings.ChromosomeNames;

        return _dbContext.Genes
            .Include(e => e.SeqRegion)
            .Include(e => e.Xref)
            .Where(e => coordSystemIds.Contains(e.SeqRegion.CoordSystemId))
            .Where(e => chromosomeNames.Contains(e.SeqRegion.Name));
    }

    private Gene Convert(Entities.Gene entity, bool length = false, bool expand = false)
    {
        if (entity != null)
        {
            var gene = new Gene(entity);

            if (length)
            {
                gene.ExonicLength = GetExonicLength(entity);
            }

            if (expand)
            {
                gene.Transcript = GetTranscript(entity, length);
            }

            return gene;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Retrieves exonic length of gene canonical transcript (SUM of length of all transcript exons).
    /// </summary>
    /// <param name="entity">Gene</param>
    /// <returns>Gene canonical transcript exonic length.</returns>
    private int GetExonicLength(Entities.Gene entity)
    {
        return _dbContext.ExonTranscripts
            .Include(e => e.Exon)
            .Where(e => e.TranscriptId == entity.CanonicalTranscriptId)
            .Sum(e => e.Exon.SeqRegionEnd - e.Exon.SeqRegionStart + 1);
    }

    /// <summary>
    /// Retrieves gene canonical transcript.
    /// </summary>
    /// <param name="entity">Gene</param>
    /// <returns>Gene canonical transcript.</returns>
    private Transcript GetTranscript(Entities.Gene entity, bool length = false)
    {
        var id = entity.CanonicalTranscriptId;

        return _transcriptSearchService.Get(id, length, true);
    }
}
