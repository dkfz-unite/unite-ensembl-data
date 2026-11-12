namespace Ensembl.Data.Models.Liftover.Variants;

public class SvRegion : VariantRegion
{
    public Chromosome OtherChromosome { get; set; }

    public int OtherStart { get; set; }

    public int OtherEnd { get; set; }

    public SvType Type { get; set; }

    public bool? Inverted { get; set; }
}
