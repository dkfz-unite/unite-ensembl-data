namespace Ensembl.Data.Services.Configuration.Constants;

public static class SvType
{
    public const string Duplication = "DUP";
    public const string TandemDuplication = "TDUP";
    public const string Insertion = "INS";
    public const string Deletion = "DEL";
    public const string Inversion = "INV";
    public const string IntrachromosomalTranslocation = "ITX";
    public const string InterchromosomalTranslocation = "CTX";
    public const string Complex = "COM";

    public static readonly string[] All =
    [
        Duplication,
        TandemDuplication,
        Insertion,
        Deletion,
        Inversion,
        IntrachromosomalTranslocation,
        InterchromosomalTranslocation,
        Complex
    ];

    public static readonly string[] RangeBased =
    [
        TandemDuplication,
        Insertion,
        Deletion,
        Inversion
    ];

    public static readonly string[] BreakpointBased =
    [
        IntrachromosomalTranslocation,
        InterchromosomalTranslocation,
        Complex
    ];
}
