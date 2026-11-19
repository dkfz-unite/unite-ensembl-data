using System.Text.Json.Serialization;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Models.Liftover;

[JsonConverter(typeof(EnumAliasJsonConverter<SequenceType>))]
public enum SequenceType
{
    /// <summary>
    /// Genomic sequence including introns.
    /// </summary>
    [EnumAlias("genomic", "genome")]
    Genomic,

    /// <summary>
    /// Spliced transcript sequence.
    /// </summary>
    [EnumAlias("cdna", "transcript")]
    Cdna,

    /// <summary>
    /// Coding sequence.
    /// </summary>
    [EnumAlias("cds", "coding")]
    Cds
}
