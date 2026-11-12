using Ensembl.Data.Helpers;
using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;
using Ensembl.Data.Services.Configuration.Constants;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Services;

public class SmLiftoverService
{
    private readonly SequenceService _sourceSequenceService;
    private readonly SequenceService _targetSequenceService;
    private readonly CoordinateLiftoverService _coordinateLiftoverService;

    public SmLiftoverService(
        EnsemblDbContext sourceDbContext,
        EnsemblDbContext targetDbContext,
        CoordinateLiftoverService coordinateLiftoverService)
    {
        _sourceSequenceService = new SequenceService(sourceDbContext);
        _targetSequenceService = new SequenceService(targetDbContext);
        _coordinateLiftoverService = coordinateLiftoverService;
    }

    public async Task<Dictionary<int, LiftoverOutcome<SmRegion>>> Lift(Dictionary<int, SmRegion> regions)
    {
        EnrichSourceSequences(regions);

        var sourceCoordinates = regions.ToDictionary(
            item => item.Key,
            item => ToCoordinates(item.Key, item.Value));

        var mapped = await _coordinateLiftoverService.Lift(
            _sourceSequenceService.Genome,
            _targetSequenceService.Genome,
            sourceCoordinates.Values.ToArray());
            
        var mappedById = mapped.ToDictionary(region => region.Id);

        var outcomes = new Dictionary<int, LiftoverOutcome<SmRegion>>();
        var targetCoordinates = new Dictionary<int, CoordinateRegion>();
        var targetChromosomes = new Dictionary<int, Chromosome>();

        foreach (var (id, source) in regions)
        {
            if (!mappedById.TryGetValue(id, out var target))
            {
                outcomes[id] = Failed(LiftoverReason.CoordinateNotMapped);
                continue;
            }

            var sourceCoordinate = sourceCoordinates[id];
            if (!GenomeSettings.TryParseChromosome(target.Chromosome, out var chromosome) ||
                target.End - target.Start != sourceCoordinate.End - sourceCoordinate.Start)
            {
                outcomes[id] = Failed(LiftoverReason.InvalidMapping);
                continue;
            }

            targetCoordinates[id] = target;
            targetChromosomes[id] = chromosome;
        }

        var targetSequences = _targetSequenceService.GetForRegions(targetCoordinates.Values.ToArray());

        foreach (var (id, target) in targetCoordinates)
        {
            var source = regions[id];
            var expectedSequence = target.Strand.Value
                ? source.Seq
                : NucleotideSequence.ReverseComplement(source.Seq);

            if (targetSequences[id] != expectedSequence)
            {
                outcomes[id] = Failed(LiftoverReason.TargetReferenceMismatch);
                continue;
            }

            outcomes[id] = new LiftoverOutcome<SmRegion>
            {
                Region = CreateTarget(source, target, targetChromosomes[id]),
                Reason = null
            };
        }

        return outcomes;
    }

    private void EnrichSourceSequences(Dictionary<int, SmRegion> regions)
    {
        var missing = regions
            .Where(item => string.IsNullOrEmpty(item.Value.Seq))
            .ToDictionary(item => item.Key, item => ToCoordinates(item.Key, item.Value));
        var sequences = _sourceSequenceService.GetForRegions(missing.Values.ToArray());

        foreach (var (id, sequence) in sequences)
            regions[id].Seq = sequence;
    }

    private static CoordinateRegion ToCoordinates(int id, SmRegion region)
    {
        return new CoordinateRegion
        {
            Id = id,
            Chromosome = region.Chromosome.ToAliasString(),
            Start = region.SeqStart,
            End = region.SeqEnd
        };
    }

    private static SmRegion CreateTarget(SmRegion source, CoordinateRegion target, Chromosome chromosome)
    {
        var start = source.IsInsertion
            ? target.Strand.Value ? target.Start + 1 : target.Start
            : target.Start;

        return new SmRegion
        {
            Chromosome = chromosome,
            Start = start,
            End = source.IsInsertion ? start : target.End,
            Ref = Transform(source.Ref, target.Strand.Value),
            Alt = Transform(source.Alt, target.Strand.Value)
        };
    }

    private static string Transform(string sequence, bool forward)
    {
        if (string.IsNullOrEmpty(sequence))
            return null;

        return forward ? sequence : NucleotideSequence.ReverseComplement(sequence);
    }

    private static LiftoverOutcome<SmRegion> Failed(LiftoverReason reason)
    {
        return new LiftoverOutcome<SmRegion>
        {
            Region = null,
            Reason = reason
        };
    }
}
