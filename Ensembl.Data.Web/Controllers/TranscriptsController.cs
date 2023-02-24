using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/transcripts")]
[ApiController]
public class TranscriptsController : Controller
{
	private readonly TranscriptSearchService _searchService;


    public TranscriptsController(TranscriptSearchService searchService)
    {
        _searchService = searchService;
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

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool length = false, bool expand = false)
    {
        if (ids == null)
        {
            return BadRequest("Transcript IDs are not set.");
        }
        else if (ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of transcript IDs are not set.");
        }

        var models = _searchService.Find(ids.Distinct(), length, expand);

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
    public IActionResult GetByName(string symbol, bool length = false, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Transcript Symbol is not set.");
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

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool length = false, bool expand = false)
    {
        if (symbols == null)
        {
            return BadRequest("Transcript symbols are not set.");
        }
        else if (symbols.Any(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Some of transcript symbols are not set.");
        }

        var models = _searchService.FindByName(symbols.Distinct(), length, expand);

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

