using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;
using Ensembl.Data.Services.Configuration.Constants;

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

    public async Task<Dictionary<int, CnvLiftoverOutcome>> Lift(Dictionary<int, CnvRegion> regions)
    {
        var coordinates = regions.Select(item => new CoordinateRegion
        {
            Id = item.Key,
            Chromosome = item.Value.Chromosome,
            Start = item.Value.Start,
            End = item.Value.End
        }).ToArray();

        var mapped = await _coordinateLiftoverService.Lift(
            _sourceGenome,
            _targetGenome,
            coordinates,
            MinimumMatch);

        var mappedById = mapped.ToDictionary(region => region.Id);
        
        var outcomes = new Dictionary<int, CnvLiftoverOutcome>();

        foreach (var (id, source) in regions)
        {
            if (!mappedById.TryGetValue(id, out var region))
            {
                outcomes[id] = Failed(LiftoverReason.CoordinateNotMapped);
                continue;
            }

            if (!GenomeSettings.IsValidChromosome(region.Chromosome) ||
                region.Chromosome != source.Chromosome ||
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

            outcomes[id] = new CnvLiftoverOutcome
            {
                Region = new CnvRegion
                {
                    Chromosome = region.Chromosome,
                    Start = region.Start,
                    End = region.End
                },
                Reason = null
            };
        }

        return outcomes;
    }

    private static CnvLiftoverOutcome Failed(string reason)
    {
        return new CnvLiftoverOutcome
        {
            Region = null,
            Reason = reason
        };
    }
}
