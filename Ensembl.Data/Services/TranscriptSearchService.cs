using Ensembl.Data.Models;
using Ensembl.Data.Services.Configuration.Constants;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Services;

public class TranscriptSearchService
{
    private readonly EnsemblDbContext _dbContext;
    private readonly ProteinSearchService _proteinRepository;


    public TranscriptSearchService(EnsemblDbContext dbContext)
    {
        _dbContext = dbContext;
        _proteinRepository = new ProteinSearchService(dbContext);
    }

    /// <summary>
    /// Finds transcript by Ensembl stable identifier.
    /// </summary>
    /// <param name="id">Stable identifier</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found transcript.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Transcript Find(string id, bool length = false, bool expand = false)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Transcript id is missing.", nameof(id));
        }

        var entity = GetQuery().FirstOrDefault(entity => entity.StableId == id);

        return Convert(entity, length, expand);
    }

    /// <summary>
    /// Finds transcripts by their Ensembl stable identifiers.
    /// </summary>
    /// <param name="ids">Stable identifiers list</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found transcripts.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Transcript[] Find(IEnumerable<string> ids, bool length = false, bool expand = false)
    {
        if (ids == null)
        {
            throw new ArgumentException("Transcript ids are missing.", nameof(ids));
        }

        var entities = GetQuery().Where(entity => ids.Contains(entity.StableId)).ToArray();

        return entities.Select(entity => Convert(entity, length, expand)).ToArray();
    }

    /// <summary>
    /// Finds transcript by symbol.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Found transcript.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Transcript FindByName(string symbol, bool length = false, bool expand = false)
    {
        if (string.IsNullOrEmpty(symbol))
        {
            throw new ArgumentException("Transcript symbol is missing.", nameof(symbol));
        }

        var entity = GetQuery().FirstOrDefault(entity => entity.Xref.DisplayLabel == symbol);

        return Convert(entity, length, expand);
    }

    /// <summary>
    /// Finds transcripts by their symbols.
    /// </summary>
    /// <param name="symbols">Symbols</param>
    /// <param name="expand">Include child entries</param>
    /// <returns>Array of found transcripts.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Transcript[] FindByName(IEnumerable<string> symbols, bool length = false, bool expand = false)
    {
        if (symbols == null)
        {
            throw new ArgumentException("Transcript symbols are missing", nameof(symbols));
        }

        var entities = GetQuery().Where(entity => symbols.Contains(entity.Xref.DisplayLabel)).ToArray();

        return entities.Select(entity => Convert(entity, length, expand)).ToArray();
    }

    internal Transcript Get(int id, bool length = false, bool expand = false)
    {
        var entity = GetQuery().FirstOrDefault(entity => entity.TranscriptId == id);

        return Convert(entity, length, expand);
    }


    private IQueryable<Entities.Transcript> GetQuery()
    {
        var coordSystemIds = _dbContext.GetCoordSystemIds();
        var chromosomeNames = GenomeSettings.ChromosomeNames;

        return _dbContext.Transcripts
            .Include(e => e.Gene)
            .Include(e => e.SeqRegion)
            .Include(e => e.Xref)
            .Where(e => coordSystemIds.Contains(e.SeqRegion.CoordSystemId))
            .Where(e => chromosomeNames.Contains(e.SeqRegion.Name));
    }

    private Transcript Convert(Entities.Transcript entity, bool length = false, bool expand = false)
    {
        if (entity != null)
        {
            var transcript = new Transcript(entity);

            if (length)
            {
                transcript.ExonicLength = GetExonicLength(entity);
            }

            if (expand)
            {
                transcript.Protein = GetProtein(entity);
            }

            return transcript;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Retrieves exonic length of a transcript (SUM of lengths of all transcript exons).
    /// </summary>
    /// <param name="entity">Transcript</param>
    /// <returns>Transcript exonic length.</returns>
    private int GetExonicLength(Entities.Transcript entity)
    {
        return _dbContext.ExonTranscripts
            .Include(e => e.Exon)
            .Where(e => e.TranscriptId == entity.TranscriptId)
            .Sum(e => e.Exon.SeqRegionEnd - e.Exon.SeqRegionStart + 1);
    }

    /// <summary>
    /// Retrieves transcript canonical protein.
    /// </summary>
    /// <param name="entity">Transcript</param>
    /// <returns>Transcript canonical protein.</returns>
    private Protein GetProtein(Entities.Transcript entity)
    {
        var id = entity.CanonicalTranslationId;

        return id == null ? null : _proteinRepository.Get(id.Value, true);
    }
}
