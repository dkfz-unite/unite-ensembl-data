using Ensembl.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/proteins")]
[ApiController]
public class ProteinsController : Controller
{
    private readonly ProteinSearchService _searchService;


    public ProteinsController(ProteinSearchService searchService)
    {
        _searchService = searchService;
    }


    [HttpGet("{id}")]
    public IActionResult Find(string id, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Protein ID is not set.");
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

    [HttpPost("")]
    public IActionResult FindAll([FromBody] string[] ids, bool expand = false)
    {
        if (ids == null)
        {
            return BadRequest("Protein IDs are not set.");
        }
        else if (ids.Any(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Some of protein IDs are not set.");
        }

        var models = _searchService.Find(ids, expand);

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

