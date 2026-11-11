using Ensembl.Data.Services.Configuration.Constants;
using Ensembl.Data.Web.Models.Liftover.Variants;
using FluentValidation;

namespace Ensembl.Data.Web.Validators.Liftover.Variants;

public class SmValidator : AbstractValidator<Sm>
{
    public SmValidator()
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
            .GreaterThan(0)
            .When(variant => variant.End.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be a positive integer")
            .GreaterThan(variant => variant.Start)
            .When(variant => variant.Start.HasValue && variant.End.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be greater than Start");

        RuleFor(variant => variant.Ref)
            .MaximumLength(200)
            .WithMessage("Maximum length is 200")
            .Matches("^[ACGTacgt]+$")
            .When(variant => !string.IsNullOrWhiteSpace(variant.Ref), ApplyConditionTo.CurrentValidator)
            .WithMessage("Should contain only A, C, G or T");

        RuleFor(variant => variant.Alt)
            .MaximumLength(200)
            .WithMessage("Maximum length is 200")
            .Matches("^[ACGTacgt]+$")
            .When(variant => !string.IsNullOrWhiteSpace(variant.Alt), ApplyConditionTo.CurrentValidator)
            .WithMessage("Should contain only A, C, G or T");

        RuleFor(variant => variant)
            .Custom(ValidateAlleles);
    }

    private static void ValidateAlleles(Sm variant, ValidationContext<Sm> context)
    {
        var reference = variant.Ref?.Trim();
        var alternate = variant.Alt?.Trim();

        if (string.IsNullOrWhiteSpace(reference) && string.IsNullOrWhiteSpace(alternate))
        {
            context.AddFailure(nameof(Sm.Ref), "At least one of Ref and Alt must be set");
            context.AddFailure(nameof(Sm.Alt), "At least one of Ref and Alt must be set");
            return;
        }

        if (!variant.Start.HasValue || variant.Start < 1 ||
            variant.End.HasValue && variant.End <= variant.Start)
            return;

        if (string.IsNullOrWhiteSpace(reference))
        {
            if (variant.End.HasValue)
                context.AddFailure(nameof(Sm.End), "Should not be set for insertions");
            if (variant.Start == 1)
                context.AddFailure(nameof(Sm.Start), "Insertion must have a preceding reference base");
            return;
        }

        if (reference.Length > 1 && !variant.End.HasValue)
            context.AddFailure(nameof(Sm.End), "Should be set for variants spanning more than one base");
        else if ((variant.End ?? variant.Start).Value - variant.Start.Value + 1 != reference.Length)
            context.AddFailure(nameof(Sm.End), "Region length must match Ref length");

        if (!string.IsNullOrWhiteSpace(alternate))
        {
            if (alternate.Length != reference.Length)
                context.AddFailure(nameof(Sm.Alt), "Alt length must match Ref length for substitutions");
            else if (alternate.Equals(reference, StringComparison.OrdinalIgnoreCase))
                context.AddFailure(nameof(Sm.Alt), "Alt must differ from Ref");
        }
    }
}

public class SmsValidator : AbstractValidator<Sm[]>
{
    private readonly IValidator<Sm> _validator = new SmValidator();

    public SmsValidator()
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
