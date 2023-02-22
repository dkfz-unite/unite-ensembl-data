using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/genes")]
[ApiController]
public class GenesController : Controller
{
	private readonly GeneSearchService _searchService;


    public GenesController(GeneSearchService searchService)
    {
        _searchService = searchService;
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Gene ID is not set.");
        }

        var model = _searchService.Find(id, expand);

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
    public IActionResult FindAll([FromBody]string[] ids, bool expand = false)
    {
        if (ids == null)
        {
            return BadRequest("Gene IDs are not set.");
        }
        else if(ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of gene IDs are not set.");
        }

        var models = _searchService.Find(ids.Distinct(), expand);

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
    public IActionResult GetByName(string symbol, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Gene symbol is not set.");
        }

        var model = _searchService.FindByName(symbol, expand);

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
    public IActionResult FindAllByName([FromBody] string[] symbols, bool expand = false)
    {
        if (symbols == null)
        {
            return BadRequest("Gene symbols are not set.");
        }
        else if (symbols.Any(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Some of gene symbols are not set.");
        }

        var models = _searchService.FindByName(symbols.Distinct(), expand);

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

