namespace Ensembl.Data.Models.Liftover.Variants;

public class SmRegion : VariantRegion
{
    public string Ref { get; set; }

    public string Alt { get; set; }

    public string Seq { get; set; }

    public bool IsInsertion => string.IsNullOrEmpty(Ref);

    public int SeqStart => IsInsertion ? Start - 1 : Start;

    public int SeqEnd => IsInsertion ? Start - 1 : End;
}
