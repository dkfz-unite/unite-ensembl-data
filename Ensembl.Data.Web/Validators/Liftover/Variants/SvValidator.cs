using Ensembl.Data.Services.Configuration.Constants;
using Ensembl.Data.Web.Models.Liftover.Variants;
using FluentValidation;

namespace Ensembl.Data.Web.Validators.Liftover.Variants;

public class SvValidator : AbstractValidator<Sv>
{
    public SvValidator()
    {
        RuleFor(variant => variant.Chromosome)
            .NotEmpty()
            .WithMessage("Should not be empty")
            .Must(GenomeSettings.IsValidChromosome)
            .When(variant => !string.IsNullOrWhiteSpace(variant.Chromosome), ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a chromosome from 1 to 22, X, Y or MT");

        RuleFor(variant => variant.Start)
            .NotNull()
            .WithMessage("Should not be empty")
            .GreaterThan(0)
            .When(variant => variant.Start.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a positive integer");

        RuleFor(variant => variant.End)
            .NotNull()
            .WithMessage("Should not be empty")
            .GreaterThan(0)
            .When(variant => variant.End.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a positive integer");

        RuleFor(variant => variant.OtherChromosome)
            .NotEmpty()
            .WithMessage("Should not be empty")
            .Must(GenomeSettings.IsValidChromosome)
            .When(variant => !string.IsNullOrWhiteSpace(variant.OtherChromosome), ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a chromosome from 1 to 22, X, Y or MT");

        RuleFor(variant => variant.OtherStart)
            .NotNull()
            .WithMessage("Should not be empty")
            .GreaterThan(0)
            .When(variant => variant.OtherStart.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a positive integer");

        RuleFor(variant => variant.OtherEnd)
            .NotNull()
            .WithMessage("Should not be empty")
            .GreaterThan(0)
            .When(variant => variant.OtherEnd.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a positive integer");

        RuleFor(variant => variant.Type)
            .NotEmpty()
            .WithMessage("Should not be empty")
            .Must(type => SvType.All.Contains(type))
            .When(variant => !string.IsNullOrWhiteSpace(variant.Type), ApplyConditionTo.CurrentValidator)
            .WithMessage($"Should be one of: {string.Join(", ", SvType.All)}");

        RuleFor(variant => variant)
            .Custom(ValidateCoordinates);
    }

    private static void ValidateCoordinates(Sv variant, ValidationContext<Sv> context)
    {
        if (!variant.Start.HasValue || !variant.End.HasValue ||
            !variant.OtherStart.HasValue || !variant.OtherEnd.HasValue ||
            string.IsNullOrEmpty(variant.Type))
            return;

        if (variant.Type == SvType.Duplication)
        {
            if (variant.Start == 1)
                context.AddFailure(nameof(Sv.Start), "Copied region must have a preceding reference base");
            if (variant.End <= variant.Start)
                context.AddFailure(nameof(Sv.End), "Should be greater than Start for a duplication source region");
        }
        else if (variant.End != variant.Start + 1)
        {
            context.AddFailure(nameof(Sv.End), "Should equal Start + 1 for an exact breakpoint");
        }

        if (variant.OtherEnd != variant.OtherStart + 1)
            context.AddFailure(nameof(Sv.OtherEnd), "Should equal OtherStart + 1 for an exact breakpoint");

        if (SvType.RangeBased.Contains(variant.Type))
        {
            if (variant.Chromosome != variant.OtherChromosome)
                context.AddFailure(nameof(Sv.OtherChromosome), "Should match Chromosome for a range-based structural variant");
            if (variant.Start == variant.OtherStart)
                context.AddFailure(nameof(Sv.OtherStart), "Should differ from Start for a range-based structural variant");
        }

        if (variant.Type == SvType.IntrachromosomalTranslocation &&
            variant.Chromosome != variant.OtherChromosome)
            context.AddFailure(nameof(Sv.OtherChromosome), "Should match Chromosome for an intrachromosomal translocation");

        if (variant.Type == SvType.InterchromosomalTranslocation &&
            variant.Chromosome == variant.OtherChromosome)
            context.AddFailure(nameof(Sv.OtherChromosome), "Should differ from Chromosome for an interchromosomal translocation");

        if (variant.Type == SvType.IntrachromosomalTranslocation &&
            variant.Start == variant.OtherStart)
            context.AddFailure(nameof(Sv.OtherStart), "Should differ from Start for an intrachromosomal translocation");

        if (variant.Type == SvType.Complex &&
            variant.Chromosome == variant.OtherChromosome &&
            variant.Start == variant.OtherStart)
            context.AddFailure(nameof(Sv.OtherStart), "Should identify a different cut for a complex rearrangement");
    }
}

public class SvsValidator : AbstractValidator<Sv[]>
{
    private readonly IValidator<Sv> _validator = new SvValidator();

    public SvsValidator()
    {
        RuleFor(variants => variants)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleForEach(variants => variants)
            .NotNull()
            .WithMessage("Should not contain empty values")
            .SetValidator(_validator);
    }
}
