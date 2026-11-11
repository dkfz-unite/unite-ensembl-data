using Ensembl.Data.Services;
using Ensembl.Data.Web.Models.Liftover;
using Ensembl.Data.Web.Models.Liftover.Variants;
using Microsoft.AspNetCore.Mvc;

namespace Ensembl.Data.Web.Controllers;

public abstract class LiftoverControllerBase : Controller
{
    private readonly SmLiftoverService _smLiftoverService;
    private readonly CnvLiftoverService _cnvLiftoverService;

    protected LiftoverControllerBase(
        EnsemblDbContext sourceContext,
        EnsemblDbContext targetContext,
        CoordinateLiftoverService coordinateLiftover)
    {
        _smLiftoverService = new SmLiftoverService(sourceContext, targetContext, coordinateLiftover);
        _cnvLiftoverService = new CnvLiftoverService(sourceContext, targetContext, coordinateLiftover);
    }

    [HttpPost("sms")]
    public async Task<IActionResult> LiftSms([FromBody] Sm[] variants)
    {
        var regions = variants
            .Select((variant, index) => new { Id = index, Region = variant.ToRegion() })
            .ToDictionary(item => item.Id, item => item.Region);

        var outcomes = await _smLiftoverService.Lift(regions);

        var results = variants.Select((variant, index) =>
        {
            var outcome = outcomes[index];
            
            return new LiftoverResult<Sm>
            {
                Source = variant,
                Target = outcome.Region == null ? null : Sm.FromRegion(outcome.Region),
                Reason = outcome.Reason
            };
        });

        return Json(results);
    }

    [HttpPost("cnvs")]
    public async Task<IActionResult> LiftCnvs([FromBody] Cnv[] variants)
    {
        var regions = variants
            .Select((variant, index) => new { Id = index, Region = variant.ToRegion() })
            .ToDictionary(item => item.Id, item => item.Region);

        var outcomes = await _cnvLiftoverService.Lift(regions);

        var results = variants.Select((variant, index) =>
        {
            var outcome = outcomes[index];

            return new LiftoverResult<Cnv>
            {
                Source = variant,
                Target = outcome.Region == null ? null : Cnv.FromRegion(outcome.Region),
                Reason = outcome.Reason
            };
        });

        return Json(results);
    }
}

[ApiController]
[Route("api/grch37/liftover/grch38")]
public sealed class Liftover37To38Controller : LiftoverControllerBase
{
    public Liftover37To38Controller(
        EnsemblDbContext37 sourceContext,
        EnsemblDbContext38 targetContext,
        CoordinateLiftoverService coordinateLiftover)
        : base(sourceContext, targetContext, coordinateLiftover)
    {
    }
}

[ApiController]
[Route("api/grch38/liftover/grch37")]
public sealed class Liftover38To37Controller : LiftoverControllerBase
{
    public Liftover38To37Controller(
        EnsemblDbContext38 sourceContext,
        EnsemblDbContext37 targetContext,
        CoordinateLiftoverService coordinateLiftover)
        : base(sourceContext, targetContext, coordinateLiftover)
    {
    }
}
