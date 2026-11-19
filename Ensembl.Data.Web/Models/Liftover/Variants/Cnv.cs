using System.Text.Json.Serialization;
using Ensembl.Data.Models.Liftover;
using Ensembl.Data.Models.Liftover.Variants;

namespace Ensembl.Data.Web.Models.Liftover.Variants;

public class Cnv
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;

    [JsonPropertyName("chromosome")]
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    [JsonPropertyName("start")]
    public int? Start { get => _start; set => _start = value; }

    [JsonPropertyName("end")]
    public int? End { get => _end; set => _end = value; }

    public CnvRegion ToRegion()
    {
        return new CnvRegion
        {
            Chromosome = Chromosome.Value,
            Start = Start.Value,
            End = End.Value
        };
    }

    public static Cnv FromRegion(CnvRegion region)
    {
        return new Cnv
        {
            Chromosome = region.Chromosome,
            Start = region.Start,
            End = region.End
        };
    }
}
