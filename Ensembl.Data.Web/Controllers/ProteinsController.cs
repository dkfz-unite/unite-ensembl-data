using Ensembl.Data.Services;
using Ensembl.Data.Web.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

[Route("api/proteins")]
[ApiController]
public class ProteinsController : Controller
{
    private const byte DefaultGRCh = 37;

    private readonly ProteinSearchService _searchService37;
    private readonly ProteinSearchService _searchService38;
    private readonly SequenceService _sequenceService37;
    private readonly SequenceService _sequenceService38;


    public ProteinsController(EnsemblDbContext37 dbContext37, EnsemblDbContext38 dbContext38)
    {
        if (dbContext37.Database.CanConnect())
        {
            _searchService37 = new ProteinSearchService(dbContext37);
            _sequenceService37 = new SequenceService(dbContext37);
        }
        
        if (dbContext38.Database.CanConnect())
        {
            _searchService38 = new ProteinSearchService(dbContext38);
            _sequenceService38 = new SequenceService(dbContext38);
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

        var model = searchService.Find(id.Trim(), expand);

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
    public IActionResult FindSequence(string id, byte grch = DefaultGRCh)
    {
        if (!TryResolveServices(grch, out var searchService, out var sequenceService))
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Protein ID is not set.");

        var protein = searchService.Find(id.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("id")]
    public IActionResult FindAll([FromBody] string[] ids, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (ids == null || ids.All(id => string.IsNullOrWhiteSpace(id)))
        {
            return BadRequest("Protein IDs are not set.");
        }

        var models = searchService.Find(ids.FilteredDistinct(), expand);

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
    public IActionResult FindByAccession(string accession, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(accession))
        {
            return BadRequest("Protein accession is not set.");
        }

        var model = searchService.FindByAccession(accession.Trim(), expand);

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
    public IActionResult FindSequenceByAccession(string accession, byte grch = DefaultGRCh)
    {
        if (!TryResolveServices(grch, out var searchService, out var sequenceService))
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");

        if (string.IsNullOrWhiteSpace(accession))
            return BadRequest("Protein accession is not set.");

        var protein = searchService.FindByAccession(accession.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("acc")]
    public IActionResult FindAllByAccession([FromBody] string[] accessions, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (accessions == null || accessions.All(acc => string.IsNullOrWhiteSpace(acc)))
        {
            return BadRequest("Protein accessions are not set.");
        }

        var models = searchService.FindByAccession(accessions.FilteredDistinct(), expand);

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
    public IActionResult FindByName(string symbol, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Protein symbol is not set.");
        }

        var model = searchService.FindByName(symbol.Trim(), expand);

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
    public IActionResult FindSequenceByName(string symbol, byte grch = DefaultGRCh)
    {
        if (!TryResolveServices(grch, out var searchService, out var sequenceService))
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");

        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Protein symbol is not set.");

        var protein = searchService.FindByName(symbol.Trim().ToLowerInvariant());
        if (protein == null)
            return NotFound();

        var sequence = sequenceService.GetForProtein(protein);
        return Json(sequence);
    }

    [HttpPost("symbol")]
    public IActionResult FindAllByName([FromBody] string[] symbols, bool expand = false, byte grch = DefaultGRCh)
    {
        if (!TryResolveSearchService(grch, out var searchService))
        {
            return BadRequest("Invalid GRCh version specified or database doesn't exist.");
        }

        if (symbols == null || symbols.All(symbol => string.IsNullOrWhiteSpace(symbol)))
        {
            return BadRequest("Protein symbols are not set.");
        }

        var models = searchService.FindByName(symbols.FilteredDistinct(), expand);

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

    private bool TryResolveServices(int grch, out ProteinSearchService searchService, out SequenceService sequenceService)
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
