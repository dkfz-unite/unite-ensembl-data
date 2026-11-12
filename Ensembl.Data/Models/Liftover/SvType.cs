using System.Text.Json.Serialization;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Models.Liftover;

[JsonConverter(typeof(EnumAliasJsonConverter<SvType>))]
public enum SvType
{
    /// <summary>
    /// Dispersed duplication.
    /// </summary>
    [EnumAlias("DUP", "duplication")]
    DUP = 1,

    /// <summary>
    /// Tandem duplication.
    /// </summary>
    [EnumAlias("TDUP", "tandem duplication", "tandem_duplication")]
    TDUP = 2,

    /// <summary>
    /// Insertion.
    /// </summary>
    [EnumAlias("INS", "insertion")]
    INS = 3,

    /// <summary>
    /// Deletion.
    /// </summary>
    [EnumAlias("DEL", "deletion")]
    DEL = 4,

    /// <summary>
    /// Inversion.
    /// </summary>
    [EnumAlias("INV", "inversion")]
    INV = 5,

    /// <summary>
    /// Intrachromosomal translocation.
    /// </summary>
    [EnumAlias("ITX", "intrachromosomal translocation", "intra-chromosomal translocation")]
    ITX = 6,

    /// <summary>
    /// Interchromosomal translocation.
    /// </summary>
    [EnumAlias("CTX", "interchromosomal translocation", "inter-chromosomal translocation")]
    CTX = 7,

    /// <summary>
    /// Complex rearrangement.
    /// </summary>
    [EnumAlias("COM", "complex", "complex rearrangement")]
    COM = 8
}
