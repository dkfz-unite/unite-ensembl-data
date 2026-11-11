namespace Ensembl.Data.Services.Configuration.Constants;

public static class LiftoverReason
{
    public const string CoordinateNotMapped = "coordinate_not_mapped";
    public const string TargetReferenceMismatch = "target_reference_mismatch";
    public const string LengthChangeExceeded = "length_change_exceeded";
    public const string InvalidMapping = "invalid_mapping";
}
