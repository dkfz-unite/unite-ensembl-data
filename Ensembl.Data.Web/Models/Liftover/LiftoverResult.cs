using System.Text.Json.Serialization;
using Ensembl.Data.Models.Liftover;

namespace Ensembl.Data.Web.Models.Liftover;

public class LiftoverResult<T>
{
    [JsonPropertyName("source")]
    public T Source { get; init; }

    [JsonPropertyName("target")]
    public T Target { get; init; }

    [JsonPropertyName("reason")]
    public LiftoverReason? Reason { get; init; }
}
