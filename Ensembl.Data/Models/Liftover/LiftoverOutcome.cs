using Ensembl.Data.Models.Liftover.Variants;

namespace Ensembl.Data.Models.Liftover;

public class LiftoverOutcome<T> where T : VariantRegion
{
    public T Region { get; set; }

    public LiftoverReason? Reason { get; set; }
}
