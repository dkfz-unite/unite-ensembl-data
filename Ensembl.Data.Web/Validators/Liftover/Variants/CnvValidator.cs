using Ensembl.Data.Services.Configuration.Constants;
using Ensembl.Data.Web.Models.Liftover.Variants;
using FluentValidation;

namespace Ensembl.Data.Web.Validators.Liftover.Variants;

public class CnvValidator : AbstractValidator<Cnv>
{
    public CnvValidator()
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
            .WithMessage("Should be a positive integer")
            .GreaterThan(variant => variant.Start)
            .When(variant => variant.Start.HasValue && variant.End.HasValue, ApplyConditionTo.CurrentValidator)
            .WithMessage("Should be greater than Start");
    }
}

public class CnvsValidator : AbstractValidator<Cnv[]>
{
    private readonly IValidator<Cnv> _validator = new CnvValidator();

    public CnvsValidator()
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
