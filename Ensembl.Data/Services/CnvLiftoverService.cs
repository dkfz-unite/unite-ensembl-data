using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;
using Ensembl.Data.Services.Configuration.Constants;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Services;

public class CnvLiftoverService
{
    private const double MinimumMatch = 0.95;
    private const double MinimumLengthRatio = 0.5;
    private const double MaximumLengthRatio = 2.0;

    private readonly byte _sourceGenome;
    private readonly byte _targetGenome;
    private readonly CoordinateLiftoverService _coordinateLiftoverService;

    public CnvLiftoverService(
        EnsemblDbContext sourceDbContext,
        EnsemblDbContext targetDbContext,
        CoordinateLiftoverService coordinateLiftoverService)
    {
        _sourceGenome = sourceDbContext.Genome;
        _targetGenome = targetDbContext.Genome;
        _coordinateLiftoverService = coordinateLiftoverService;
    }

    public async Task<Dictionary<int, LiftoverOutcome<CnvRegion>>> Lift(Dictionary<int, CnvRegion> regions)
    {
        var coordinates = regions.Select(item => new CoordinateRegion
        {
            Id = item.Key,
            Chromosome = item.Value.Chromosome.ToAliasString(),
            Start = item.Value.Start,
            End = item.Value.End
        }).ToArray();

        var mapped = await _coordinateLiftoverService.Lift(
            _sourceGenome,
            _targetGenome,
            coordinates,
            MinimumMatch);

        var mappedById = mapped.ToDictionary(region => region.Id);
        
        var outcomes = new Dictionary<int, LiftoverOutcome<CnvRegion>>();

        foreach (var (id, source) in regions)
        {
            if (!mappedById.TryGetValue(id, out var region))
            {
                outcomes[id] = Failed(LiftoverReason.CoordinateNotMapped);
                continue;
            }

            if (!GenomeSettings.TryParseChromosome(region.Chromosome, out var chromosome) ||
                chromosome != source.Chromosome ||
                region.End <= region.Start)
            {
                outcomes[id] = Failed(LiftoverReason.InvalidMapping);
                continue;
            }

            var sourceLength = source.End - source.Start + 1;
            var targetLength = region.End - region.Start + 1;
            var lengthRatio = (double)targetLength / sourceLength;

            if (lengthRatio <= MinimumLengthRatio || lengthRatio >= MaximumLengthRatio)
            {
                outcomes[id] = Failed(LiftoverReason.LengthChangeExceeded);
                continue;
            }

            outcomes[id] = new LiftoverOutcome<CnvRegion>
            {
                Region = new CnvRegion
                {
                    Chromosome = chromosome,
                    Start = region.Start,
                    End = region.End
                },
                Reason = null
            };
        }

        return outcomes;
    }

    private static LiftoverOutcome<CnvRegion> Failed(LiftoverReason reason)
    {
        return new LiftoverOutcome<CnvRegion>
        {
            Region = null,
            Reason = reason
        };
    }
}
