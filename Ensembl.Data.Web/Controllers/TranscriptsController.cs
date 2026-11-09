using Ensembl.Data.Services;
using Ensembl.Data.Services.Configuration.Constants;
using Ensembl.Data.Web.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/transcripts")]
[ApiController]
public class TranscriptsController : Controller
{
    private const byte DefaultGRCh = 37;

    private readonly TranscriptSearchService _searchService37;
    private readonly TranscriptSearchService _searchService38;
    private readonly SequenceService _sequenceService37;
    private readonly SequenceService _sequenceService38;


    public TranscriptsController(EnsemblDbContext37 dbContext37, EnsemblDbContext38 dbContext38)
    {
        if (dbContext37.Database.CanConnect())
        {
            _searchService37 = new TranscriptSearchService(dbContext37);
            _sequenceService37 = new SequenceService(dbContext37);
        }
        
        if (dbContext38.Database.CanConnect())
        {
            _searchService38 = new TranscriptSearchService(dbContext38);
            _sequenceService38 = new SequenceService(dbContext38);
        }
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Transcript ID is not set.");
        }

        var model = searchService.Find(id, length, expand);

        if (model != null)
        {
            return Json(model);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("id/{id}/sequence")]
    public IActionResult FindSequence(string id, byte grch = DefaultGRCh, string type = SequenceType.Genomic)
    {
        if (!TryResolveServices(grch, out var searchService, out var sequenceService))
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Transcript ID is not set.");

        type = type?.Trim().ToLowerInvariant();
        if (type is not (SequenceType.Genomic or SequenceType.Cdna or SequenceType.Cds))
            return BadRequest("Transcript sequence type must be genomic, cdna or cds.");

        var transcript = searchService.Find(id.Trim().ToLowerInvariant());
        if (transcript == null)
            return NotFound();

        var sequence = sequenceService.GetForTranscript(transcript, type);
        return Json(sequence);
    }

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (ids == null || ids.All(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Transcript IDs are not set.");
        }

        var models = searchService.Find(ids.FilteredDistinct(), length, expand);

        if (models != null)
        {
            return Json(models);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("symbol/{symbol}")]
    public IActionResult FindByName(string symbol, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Transcript symbol is not set.");
        }

        var model = searchService.FindByName(symbol, length, expand);

        if (model != null)
        {
            return Json(model);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("symbol/{symbol}/sequence")]
    public IActionResult FindSequenceByName(string symbol, byte grch = DefaultGRCh, string type = SequenceType.Genomic)
    {
        if (!TryResolveServices(grch, out var searchService, out var sequenceService))
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");

        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Transcript symbol is not set.");

        type = type?.Trim().ToLowerInvariant();
        if (type is not (SequenceType.Genomic or SequenceType.Cdna or SequenceType.Cds))
            return BadRequest("Transcript sequence type must be genomic, cdna or cds.");

        var transcript = searchService.FindByName(symbol.Trim().ToLowerInvariant());
        if (transcript == null)
            return NotFound();

        var sequence = sequenceService.GetForTranscript(transcript, type);
        return Json(sequence);
    }

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (symbols == null || symbols.All(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Transcript symbols are not set.");
        }

        var models = searchService.FindByName(symbols.FilteredDistinct(), length, expand);

        if (models != null)
        {
            return Json(models);
        }
        else
        {
            return NotFound();
        }
    }


    private bool TryResolveSearchService(int grch, out TranscriptSearchService searchService)
    {
        searchService = grch switch
        {
            37 => _searchService37,
            38 => _searchService38,
            _ => null
        };

        return searchService != null;
    }

    private bool TryResolveServices(int grch, out TranscriptSearchService searchService, out SequenceService sequenceService)
    {
        TryResolveSearchService(grch, out searchService);
        sequenceService = grch switch
        {
            37 => _sequenceService37,
            38 => _sequenceService38,
            _ => null
        };

        return searchService != null && sequenceService != null;
    }
}
