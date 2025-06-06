using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/transcripts")]
[ApiController]
public class TranscriptsController : Controller
{
    private const byte DefaultGRCh = 37;

    private readonly TranscriptSearchService _searchService37;
    private readonly TranscriptSearchService _searchService38;


    public TranscriptsController(EnsemblDbContext37 dbContext37, EnsemblDbContext38 dbContext38)
    {
        if (dbContext37.Database.CanConnect())
        {
            _searchService37 = new TranscriptSearchService(dbContext37);
        }
        
        if (dbContext38.Database.CanConnect())
        {
            _searchService38 = new TranscriptSearchService(dbContext38);
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

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (ids == null)
        {
            return BadRequest("Transcript IDs are not set.");
        }
        else if (ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of transcript IDs are not set.");
        }

        var models = searchService.Find(ids.Distinct(), length, expand);

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
    public IActionResult GetByName(string symbol, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Transcript Symbol is not set.");
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

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool length = false, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (symbols == null)
        {
            return BadRequest("Transcript symbols are not set.");
        }
        else if (symbols.Any(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Some of transcript symbols are not set.");
        }

        var models = searchService.FindByName(symbols.Distinct(), length, expand);

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
}
