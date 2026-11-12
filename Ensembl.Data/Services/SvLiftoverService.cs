using Ensembl.Data.Helpers;
using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;
using Ensembl.Data.Services.Configuration.Constants;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Services;

public class SvLiftoverService
{
    private const double MinimumRangeMatch = 0.95;
    private const double MinimumLengthRatio = 0.5;
    private const double MaximumLengthRatio = 2.0;

    private readonly SequenceService _sourceSequenceService;
    private readonly SequenceService _targetSequenceService;
    private readonly CoordinateLiftoverService _coordinateLiftoverService;

    public SvLiftoverService(
        EnsemblDbContext sourceDbContext,
        EnsemblDbContext targetDbContext,
        CoordinateLiftoverService coordinateLiftoverService)
    {
        _sourceSequenceService = new SequenceService(sourceDbContext);
        _targetSequenceService = new SequenceService(targetDbContext);
        _coordinateLiftoverService = coordinateLiftoverService;
    }

    public async Task<Dictionary<int, LiftoverOutcome<SvRegion>>> Lift(Dictionary<int, SvRegion> regions)
    {
        var outcomes = new Dictionary<int, LiftoverOutcome<SvRegion>>();

        var rangeBased = regions
            .Where(item => item.Value.Type.IsRangeBased())
            .ToDictionary();

        var breakpointBased = regions
            .Where(item => item.Value.Type.IsBreakpointBased())
            .ToDictionary();
            
        var duplications = regions
            .Where(item => item.Value.Type == SvType.DUP)
            .ToDictionary();

        await LiftRangeBased(rangeBased, outcomes);
        await LiftBreakpointBased(breakpointBased, outcomes);
        await LiftDuplications(duplications, outcomes);

        return outcomes;
    }

    private async Task LiftRangeBased(
        Dictionary<int, SvRegion> regions,
        Dictionary<int, LiftoverOutcome<SvRegion>> outcomes)
    {
        if (regions.Count == 0)
            return;

        var breakpoints = new Dictionary<int, CoordinateRegion>();
        var ranges = new Dictionary<int, CoordinateRegion>();

        foreach (var (id, region) in regions)
        {
            breakpoints[id * 2] = CreateBreakpoint(id * 2, region.Chromosome, region.Start);
            breakpoints[id * 2 + 1] = CreateBreakpoint(id * 2 + 1, region.OtherChromosome, region.OtherStart);
            ranges[id] = CreateRange(id, region.Chromosome, region.Start, region.OtherStart);
        }

        var mappedBreakpoints = await LiftBreakpoints(breakpoints);
        var mappedRanges = await LiftRanges(ranges);

        foreach (var (id, source) in regions)
        {
            var first = mappedBreakpoints[id * 2];
            var second = mappedBreakpoints[id * 2 + 1];
            var range = mappedRanges[id];
            var reason = FirstFailure(first.Reason, second.Reason, range.Reason);

            if (reason != null)
            {
                outcomes[id] = Failed(reason.Value);
                continue;
            }

            var expectedStart = Math.Min(first.Region.Start, second.Region.Start) + 1;
            var expectedEnd = Math.Max(first.Region.Start, second.Region.Start);

            if (first.Region.Chromosome != second.Region.Chromosome ||
                range.Region.Chromosome != first.Region.Chromosome ||
                range.Region.Start != expectedStart ||
                range.Region.End != expectedEnd ||
                first.Region.Strand != range.Region.Strand ||
                second.Region.Strand != range.Region.Strand)
            {
                outcomes[id] = Failed(LiftoverReason.InvalidMapping);
                continue;
            }

            outcomes[id] = Succeeded(new SvRegion
            {
                Chromosome = first.Region.Chromosome.FromAliasString<Chromosome>(),
                Start = first.Region.Start,
                End = first.Region.End,
                OtherChromosome = second.Region.Chromosome.FromAliasString<Chromosome>(),
                OtherStart = second.Region.Start,
                OtherEnd = second.Region.End,
                Type = source.Type,
                Inverted = TransformInverted(
                    source.Inverted,
                    first.Region.Strand != second.Region.Strand)
            });
        }
    }

    private async Task LiftBreakpointBased(
        Dictionary<int, SvRegion> regions,
        Dictionary<int, LiftoverOutcome<SvRegion>> outcomes)
    {
        if (regions.Count == 0)
            return;

        var breakpoints = new Dictionary<int, CoordinateRegion>();
        foreach (var (id, region) in regions)
        {
            breakpoints[id * 2] = CreateBreakpoint(id * 2, region.Chromosome, region.Start);
            breakpoints[id * 2 + 1] = CreateBreakpoint(id * 2 + 1, region.OtherChromosome, region.OtherStart);
        }

        var mapped = await LiftBreakpoints(breakpoints);

        foreach (var (id, source) in regions)
        {
            var first = mapped[id * 2];
            var second = mapped[id * 2 + 1];
            var reason = FirstFailure(first.Reason, second.Reason);

            if (reason != null)
            {
                outcomes[id] = Failed(reason.Value);
                continue;
            }

            var targetSameChromosome = first.Region.Chromosome == second.Region.Chromosome;
            var topologyIsValid = source.Type switch
            {
                SvType.ITX => targetSameChromosome,
                SvType.CTX => !targetSameChromosome,
                SvType.COM => targetSameChromosome == (source.Chromosome == source.OtherChromosome),
                _ => false
            };

            if (!topologyIsValid)
            {
                outcomes[id] = Failed(LiftoverReason.InvalidMapping);
                continue;
            }

            outcomes[id] = Succeeded(new SvRegion
            {
                Chromosome = first.Region.Chromosome.FromAliasString<Chromosome>(),
                Start = first.Region.Start,
                End = first.Region.End,
                OtherChromosome = second.Region.Chromosome.FromAliasString<Chromosome>(),
                OtherStart = second.Region.Start,
                OtherEnd = second.Region.End,
                Type = source.Type,
                Inverted = TransformInverted(
                    source.Inverted,
                    first.Region.Strand != second.Region.Strand)
            });
        }
    }

    private async Task LiftDuplications(
        Dictionary<int, SvRegion> regions,
        Dictionary<int, LiftoverOutcome<SvRegion>> outcomes)
    {
        if (regions.Count == 0)
            return;

        var breakpoints = new Dictionary<int, CoordinateRegion>();
        var ranges = new Dictionary<int, CoordinateRegion>();

        foreach (var (id, region) in regions)
        {
            breakpoints[id * 3] = CreateBreakpoint(id * 3, region.Chromosome, region.Start - 1);
            breakpoints[id * 3 + 1] = CreateBreakpoint(id * 3 + 1, region.Chromosome, region.End);
            breakpoints[id * 3 + 2] = CreateBreakpoint(id * 3 + 2, region.OtherChromosome, region.OtherStart);
            ranges[id] = new CoordinateRegion
            {
                Id = id,
                Chromosome = region.Chromosome.ToAliasString(),
                Start = region.Start,
                End = region.End
            };
        }

        var mappedBreakpoints = await LiftBreakpoints(breakpoints);
        var mappedRanges = await LiftRanges(ranges);

        foreach (var (id, source) in regions)
        {
            var left = mappedBreakpoints[id * 3];
            var right = mappedBreakpoints[id * 3 + 1];
            var destination = mappedBreakpoints[id * 3 + 2];
            var range = mappedRanges[id];
            var reason = FirstFailure(left.Reason, right.Reason, destination.Reason, range.Reason);

            if (reason != null)
            {
                outcomes[id] = Failed(reason.Value);
                continue;
            }

            var expectedStart = Math.Min(left.Region.Start, right.Region.Start) + 1;
            var expectedEnd = Math.Max(left.Region.Start, right.Region.Start);

            if (left.Region.Chromosome != right.Region.Chromosome ||
                range.Region.Chromosome != left.Region.Chromosome ||
                range.Region.Start != expectedStart ||
                range.Region.End != expectedEnd ||
                left.Region.Strand != range.Region.Strand ||
                right.Region.Strand != range.Region.Strand)
            {
                outcomes[id] = Failed(LiftoverReason.InvalidMapping);
                continue;
            }

            outcomes[id] = Succeeded(new SvRegion
            {
                Chromosome = range.Region.Chromosome.FromAliasString<Chromosome>(),
                Start = range.Region.Start,
                End = range.Region.End,
                OtherChromosome = destination.Region.Chromosome.FromAliasString<Chromosome>(),
                OtherStart = destination.Region.Start,
                OtherEnd = destination.Region.End,
                Type = source.Type,
                Inverted = TransformInverted(
                    source.Inverted,
                    range.Region.Strand != destination.Region.Strand)
            });
        }
    }

    private async Task<Dictionary<int, (CoordinateRegion Region, LiftoverReason? Reason)>> LiftBreakpoints(
        Dictionary<int, CoordinateRegion> regions)
    {
        var sourceSequences = _sourceSequenceService.GetForRegions(regions.Values.ToArray());
        var mapped = await _coordinateLiftoverService.Lift(
            _sourceSequenceService.Genome,
            _targetSequenceService.Genome,
            regions.Values.ToArray());
        var mappedById = mapped.ToDictionary(region => region.Id);
        var outcomes = new Dictionary<int, (CoordinateRegion Region, LiftoverReason? Reason)>();
        var targetRegions = new Dictionary<int, CoordinateRegion>();

        foreach (var (id, source) in regions)
        {
            if (!mappedById.TryGetValue(id, out var target))
            {
                outcomes[id] = (null, LiftoverReason.CoordinateNotMapped);
                continue;
            }

            if (!GenomeSettings.TryParseChromosome(target.Chromosome, out _) ||
                target.Chromosome != source.Chromosome ||
                target.End - target.Start != 1 ||
                !target.Strand.HasValue)
            {
                outcomes[id] = (null, LiftoverReason.InvalidMapping);
                continue;
            }

            targetRegions[id] = target;
        }

        var targetSequences = _targetSequenceService.GetForRegions(targetRegions.Values.ToArray());

        foreach (var (id, target) in targetRegions)
        {
            var expected = target.Strand.Value
                ? sourceSequences[id]
                : NucleotideSequence.ReverseComplement(sourceSequences[id]);

            outcomes[id] = targetSequences[id] == expected
                ? (target, null)
                : (null, LiftoverReason.TargetReferenceMismatch);
        }

        return outcomes;
    }

    private async Task<Dictionary<int, (CoordinateRegion Region, LiftoverReason? Reason)>> LiftRanges(
        Dictionary<int, CoordinateRegion> regions)
    {
        var mapped = await _coordinateLiftoverService.Lift(
            _sourceSequenceService.Genome,
            _targetSequenceService.Genome,
            regions.Values.ToArray(),
            MinimumRangeMatch);
        var mappedById = mapped.ToDictionary(region => region.Id);
        var outcomes = new Dictionary<int, (CoordinateRegion Region, LiftoverReason? Reason)>();

        foreach (var (id, source) in regions)
        {
            if (!mappedById.TryGetValue(id, out var target))
            {
                outcomes[id] = (null, LiftoverReason.CoordinateNotMapped);
                continue;
            }

            if (!GenomeSettings.TryParseChromosome(target.Chromosome, out _) ||
                target.Chromosome != source.Chromosome ||
                target.End < target.Start ||
                !target.Strand.HasValue)
            {
                outcomes[id] = (null, LiftoverReason.InvalidMapping);
                continue;
            }

            var sourceLength = source.End - source.Start + 1;
            var targetLength = target.End - target.Start + 1;
            var lengthRatio = (double)targetLength / sourceLength;

            outcomes[id] = lengthRatio <= MinimumLengthRatio || lengthRatio >= MaximumLengthRatio
                ? (null, LiftoverReason.LengthChangeExceeded)
                : (target, null);
        }

        return outcomes;
    }

    private static CoordinateRegion CreateBreakpoint(int id, Chromosome chromosome, int position)
    {
        return new CoordinateRegion
        {
            Id = id,
            Chromosome = chromosome.ToAliasString(),
            Start = position,
            End = position + 1
        };
    }

    private static CoordinateRegion CreateRange(int id, Chromosome chromosome, int firstCut, int secondCut)
    {
        return new CoordinateRegion
        {
            Id = id,
            Chromosome = chromosome.ToAliasString(),
            Start = Math.Min(firstCut, secondCut) + 1,
            End = Math.Max(firstCut, secondCut)
        };
    }

    private static bool? TransformInverted(bool? inverted, bool orientationChanged)
    {
        return inverted.HasValue ? inverted.Value ^ orientationChanged : null;
    }

    private static LiftoverReason? FirstFailure(params LiftoverReason?[] reasons)
    {
        return reasons.FirstOrDefault(reason => reason.HasValue);
    }

    private static LiftoverOutcome<SvRegion> Succeeded(SvRegion region)
    {
        return new LiftoverOutcome<SvRegion>
        {
            Region = region,
            Reason = null
        };
    }

    private static LiftoverOutcome<SvRegion> Failed(LiftoverReason reason)
    {
        return new LiftoverOutcome<SvRegion>
        {
            Region = null,
            Reason = reason
        };
    }

}
