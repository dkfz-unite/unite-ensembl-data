namespace Ensembl.Data.Models.Liftover.Variants;

public class SvRegion
{
    public string Chromosome { get; set; }

    public int Start { get; set; }

    public int End { get; set; }

    public string OtherChromosome { get; set; }

    public int OtherStart { get; set; }

    public int OtherEnd { get; set; }

    public string Type { get; set; }

    public bool? Inverted { get; set; }
}
