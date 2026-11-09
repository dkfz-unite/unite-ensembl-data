using Ensembl.Data.Services;
using Ensembl.Data.Services.Configuration.Constants;
using Ensembl.Data.Web.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

public abstract class TranscriptsControllerBase : Controller
{
    private readonly TranscriptSearchService _searchService;
    private readonly SequenceService _sequenceService;


    protected TranscriptsControllerBase(EnsemblDbContext dbContext)
    {
        _searchService = new TranscriptSearchService(dbContext);
        _sequenceService = new SequenceService(dbContext);
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool length = false, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Transcript ID is not set.");
        }

        var model = _searchService.Find(id, length, expand);

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
    public IActionResult FindSequence(string id, string type = SequenceType.Genomic)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Transcript ID is not set.");

        type = type?.Trim().ToLowerInvariant();
        if (type is not (SequenceType.Genomic or SequenceType.Cdna or SequenceType.Cds))
            return BadRequest("Transcript sequence type must be genomic, cdna or cds.");

        var transcript = _searchService.Find(id.Trim().ToLowerInvariant());
        if (transcript == null)
            return NotFound();

        var sequence = _sequenceService.GetForTranscript(transcript, type);
        return Json(sequence);
    }

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool length = false, bool expand = false)
    {
        if (ids == null || ids.All(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Transcript IDs are not set.");
        }

        var models = _searchService.Find(ids.FilteredDistinct(), length, expand);

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
    public IActionResult FindByName(string symbol, bool length = false, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Transcript symbol is not set.");
        }

        var model = _searchService.FindByName(symbol, length, expand);

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
    public IActionResult FindSequenceByName(string symbol, string type = SequenceType.Genomic)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Transcript symbol is not set.");

        type = type?.Trim().ToLowerInvariant();
        if (type is not (SequenceType.Genomic or SequenceType.Cdna or SequenceType.Cds))
            return BadRequest("Transcript sequence type must be genomic, cdna or cds.");

        var transcript = _searchService.FindByName(symbol.Trim().ToLowerInvariant());
        if (transcript == null)
            return NotFound();

        var sequence = _sequenceService.GetForTranscript(transcript, type);
        return Json(sequence);
    }

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool length = false, bool expand = false)
    {
        if (symbols == null || symbols.All(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Transcript symbols are not set.");
        }

        var models = _searchService.FindByName(symbols.FilteredDistinct(), length, expand);

        if (models != null)
        {
            return Json(models);
        }
        else
        {
            return NotFound();
        }
    }
}

[ApiController]
[Route("api/transcripts")]
[Route("api/grch37/transcripts")]
public sealed class Transcripts37Controller : TranscriptsControllerBase
{
    public Transcripts37Controller(EnsemblDbContext37 dbContext) : base(dbContext)
    {
    }
}

[ApiController]
[Route("api/grch38/transcripts")]
public sealed class Transcripts38Controller : TranscriptsControllerBase
{
    public Transcripts38Controller(EnsemblDbContext38 dbContext) : base(dbContext)
    {
    }
}
