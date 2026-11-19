using Ensembl.Data.Models.Liftover;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Services.Configuration.Constants;

public static class GenomeSettings
{
    public static readonly string[] ChromosomeNames = Enum
        .GetValues<Chromosome>()
        .Select(chromosome => chromosome.ToAliasString())
        .ToArray();

    public static bool IsValidChromosome(string chromosome)
    {
        return TryParseChromosome(chromosome, out _);
    }

    public static bool TryParseChromosome(string value, out Chromosome chromosome)
    {
        chromosome = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            chromosome = value.FromAliasString<Chromosome>();
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
