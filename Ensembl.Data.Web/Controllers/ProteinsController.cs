using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/proteins")]
[ApiController]
public class ProteinsController : Controller
{
    private const byte DefaultGRCh = 37;

    private readonly ProteinSearchService _searchService37;
    private readonly ProteinSearchService _searchService38;


    public ProteinsController(EnsemblDbContext37 dbContext37, EnsemblDbContext38 dbContext38)
    {
        if (dbContext37.Database.CanConnect())
        {
            _searchService37 = new ProteinSearchService(dbContext37);
        }
        
        if (dbContext38.Database.CanConnect())
        {
            _searchService38 = new ProteinSearchService(dbContext38);
        }
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Protein ID is not set.");
        }

        var model = searchService.Find(id, expand);

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
    public IActionResult FindAll([FromBody] string[] ids, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (ids == null)
        {
            return BadRequest("Protein IDs are not set.");
        }
        else if (ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of protein IDs are not set.");
        }

        var models = searchService.Find(ids.Distinct(), expand);

        if (models != null)
        {
            return Json(models);
        }
        else
        {
            return NotFound();
        }
    }


    private bool TryResolveSearchService(int grch, out ProteinSearchService searchService)
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
