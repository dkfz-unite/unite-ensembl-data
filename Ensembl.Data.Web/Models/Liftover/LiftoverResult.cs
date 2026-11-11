using System.Text.Json.Serialization;

namespace Ensembl.Data.Web.Models.Liftover;

public class LiftoverResult<T>
{
    [JsonPropertyName("source")]
    public T Source { get; init; }

    [JsonPropertyName("target")]
    public T Target { get; init; }

    [JsonPropertyName("reason")]
    public string Reason { get; init; }
}
