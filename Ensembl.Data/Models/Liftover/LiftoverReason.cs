using System.Text.Json.Serialization;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Models.Liftover;

[JsonConverter(typeof(EnumAliasJsonConverter<LiftoverReason>))]
public enum LiftoverReason
{
    /// <summary>
    /// Coordinate could not be projected to the target assembly.
    /// </summary>
    [EnumAlias("coordinate_not_mapped")]
    CoordinateNotMapped,

    /// <summary>
    /// Target reference sequence differs from the source reference sequence.
    /// </summary>
    [EnumAlias("target_reference_mismatch")]
    TargetReferenceMismatch,

    /// <summary>
    /// Target-to-source length ratio is outside the accepted range.
    /// </summary>
    [EnumAlias("length_change_exceeded")]
    LengthChangeExceeded,

    /// <summary>
    /// Mapped coordinates do not satisfy the required structure or topology.
    /// </summary>
    [EnumAlias("invalid_mapping")]
    InvalidMapping
}
