using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/genes")]
[ApiController]
public class GenesController : Controller
{
    private const byte DefaultGRCh = 37;

    private readonly GeneSearchService _searchService37;
    private readonly GeneSearchService _searchService38;

    public GenesController(EnsemblDbContext37 dbContext37, EnsemblDbContext38 dbContext38)
    {
        if (dbContext37.Database.CanConnect())
        {
            _searchService37 = new GeneSearchService(dbContext37);
        }
        
        if (dbContext38.Database.CanConnect())
        {
            _searchService38 = new GeneSearchService(dbContext38);
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
            return BadRequest("Gene ID is not set.");
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
            return BadRequest("Gene IDs are not set.");
        }
        else if (ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of gene IDs are not set.");
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
            return BadRequest("Gene symbols are not set.");
        }
        else if (symbols.Any(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Some of gene symbols are not set.");
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


    private bool TryResolveSearchService(int grch, out GeneSearchService searchService)
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
