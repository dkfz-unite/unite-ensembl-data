using System.Text.Json.Serialization;
using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;

namespace Ensembl.Data.Web.Models.Liftover.Variants;

public class Sm
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private string _ref;
    private string _alt;

    [JsonPropertyName("chromosome")]
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    [JsonPropertyName("start")]
    public int? Start { get => _start; set => _start = value; }

    [JsonPropertyName("end")]
    public int? End { get => _end; set => _end = value; }

    [JsonPropertyName("ref")]
    public string Ref { get => _ref?.Trim().ToUpper(); set => _ref = value; }

    [JsonPropertyName("alt")]
    public string Alt { get => _alt?.Trim().ToUpper(); set => _alt = value; }

    public SmRegion ToRegion()
    {
        return new SmRegion
        {
            Chromosome = Chromosome.Value,
            Start = Start.Value,
            End = End ?? Start.Value,
            Ref = Ref,
            Alt = Alt,
            Seq = Ref
        };
    }

    public static Sm FromRegion(SmRegion region)
    {
        return new Sm
        {
            Chromosome = region.Chromosome,
            Start = region.Start,
            End = region.End == region.Start ? null : region.End,
            Ref = region.Ref,
            Alt = region.Alt
        };
    }
}
