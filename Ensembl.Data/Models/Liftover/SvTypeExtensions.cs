namespace Ensembl.Data.Models.Liftover;

public static class SvTypeExtensions
{
    /// <summary>
    /// Indicates whether the structural variant describes a continuous source range.
    /// </summary>
    public static bool IsRangeBased(this SvType type)
    {
        return type is SvType.TDUP or SvType.INS or SvType.DEL or SvType.INV;
    }

    /// <summary>
    /// Indicates whether the structural variant is represented by two independent breakpoints.
    /// </summary>
    public static bool IsBreakpointBased(this SvType type)
    {
        return type is SvType.ITX or SvType.CTX or SvType.COM;
    }
}
