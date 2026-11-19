using Ensembl.Data.Services;
using Ensembl.Data.Web.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

public abstract class GenesControllerBase : Controller
{
    private readonly GeneSearchService _searchService;
    private readonly SequenceService _sequenceService;


    protected GenesControllerBase(EnsemblDbContext dbContext)
    {
        _searchService = new GeneSearchService(dbContext);
        _sequenceService = new SequenceService(dbContext);
    }


    [HttpGet("id/{id}")]
    public IActionResult Find(string id, bool length = false, bool expand = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Gene ID is not set.");
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
    public IActionResult FindSequence(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Gene ID is not set.");

        var gene = _searchService.Find(id.Trim().ToLowerInvariant());
        if (gene == null)
            return NotFound();

        var sequence = _sequenceService.GetForGene(gene);
        return Json(sequence);
    }

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool length = false, bool expand = false)
    {
        if (ids == null || ids.All(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Gene IDs are not set.");
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
    public IActionResult FindSequenceByName(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Gene symbol is not set.");

        var gene = _searchService.FindByName(symbol.Trim().ToLowerInvariant());
        if (gene == null)
            return NotFound();

        var sequence = _sequenceService.GetForGene(gene);
        return Json(sequence);
    }

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool length = false, bool expand = false)
    {
        if (symbols == null || symbols.All(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Gene symbols are not set.");
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
[Route("api/genes")]
[Route("api/grch37/genes")]
public sealed class Genes37Controller : GenesControllerBase
{
    public Genes37Controller(EnsemblDbContext37 dbContext) : base(dbContext)
    {
    }
}

[ApiController]
[Route("api/grch38/genes")]
public sealed class Genes38Controller : GenesControllerBase
{
    public Genes38Controller(EnsemblDbContext38 dbContext) : base(dbContext)
    {
    }
}
