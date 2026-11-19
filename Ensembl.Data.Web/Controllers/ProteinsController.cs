using Ensembl.Data.Services;
using Ensembl.Data.Web.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

public abstract class ProteinsControllerBase : Controller
{
    private readonly ProteinSearchService _searchService;
    private readonly SequenceService _sequenceService;


    protected ProteinsControllerBase(EnsemblDbContext dbContext)
    {
        _searchService = new ProteinSearchService(dbContext);
        _sequenceService = new SequenceService(dbContext);
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Protein ID is not set.");
        }

        var model = _searchService.Find(id.Trim(), expand);

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
    public IActionResult FindSequence(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Protein ID is not set.");

        var protein = _searchService.Find(id.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = _sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool expand = false)
    {
        if (ids == null || ids.All(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Protein IDs are not set.");
        }

        var models = _searchService.Find(ids.FilteredDistinct(), expand);

        if (models != null)
        {
            return Json(models);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("acc/{accession}")]
    public IActionResult FindByAccession(string accession, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(accession))
        {
            return BadRequest("Protein accession is not set.");
        }

        var model = _searchService.FindByAccession(accession.Trim(), expand);

        if (model != null)
        {
            return Json(model);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("acc/{accession}/sequence")]
    public IActionResult FindSequenceByAccession(string accession)
    {
        if (string.IsNullOrWhiteSpace(accession))
            return BadRequest("Protein accession is not set.");

        var protein = _searchService.FindByAccession(accession.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = _sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("acc")]
    public IActionResult FindAllByAccession([FromBody] string[] accessions, bool expand = false)
    {
        if (accessions == null || accessions.All(acc => string.IsNullOrWhiteSpace(acc)))
        {
            return BadRequest("Protein accessions are not set.");
        }

        var models = _searchService.FindByAccession(accessions.FilteredDistinct(), expand);

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
    public IActionResult FindByName(string symbol, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Protein symbol is not set.");
        }

        var model = _searchService.FindByName(symbol.Trim(), expand);

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
    public IActionResult FindSequenceByName(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Protein symbol is not set.");

        var protein = _searchService.FindByName(symbol.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = _sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool expand = false)
    {
        if (symbols == null || symbols.All(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Protein symbols are not set.");
        }

        var models = _searchService.FindByName(symbols.FilteredDistinct(), expand);

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
[Route("api/proteins")]
[Route("api/grch37/proteins")]
public sealed class Proteins37Controller : ProteinsControllerBase
{
    public Proteins37Controller(EnsemblDbContext37 dbContext) : base(dbContext)
    {
    }
}

[ApiController]
[Route("api/grch38/proteins")]
public sealed class Proteins38Controller : ProteinsControllerBase
{
    public Proteins38Controller(EnsemblDbContext38 dbContext) : base(dbContext)
    {
    }
}
