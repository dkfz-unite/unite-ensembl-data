using System.Text.Json.Serialization;
using Ensembl.Data.Models.Liftover.Variants;
using Ensembl.Data.Services.Configuration.Constants;

namespace Ensembl.Data.Web.Models.Liftover.Variants;

public class Sv
{
    private string _chromosome;
    private int? _start;
    private int? _end;
    private string _otherChromosome;
    private int? _otherStart;
    private int? _otherEnd;
    private string _type;
    private bool? _inverted;

    [JsonPropertyName("chromosome_1")]
    public string Chromosome { get => _chromosome?.Trim().ToUpper(); set => _chromosome = value; }

    [JsonPropertyName("start_1")]
    public int? Start { get => _start; set => _start = value; }

    [JsonPropertyName("end_1")]
    public int? End { get => _end ?? (Type == SvType.Duplication ? null : _start + 1); set => _end = value; }

    [JsonPropertyName("chromosome_2")]
    public string OtherChromosome { get => _otherChromosome?.Trim().ToUpper(); set => _otherChromosome = value; }

    [JsonPropertyName("start_2")]
    public int? OtherStart { get => _otherStart; set => _otherStart = value; }

    [JsonPropertyName("end_2")]
    public int? OtherEnd { get => _otherEnd ?? _otherStart + 1; set => _otherEnd = value; }

    [JsonPropertyName("type")]
    public string Type { get => _type?.Trim().ToUpper(); set => _type = value; }

    [JsonPropertyName("inverted")]
    public bool? Inverted { get => _inverted; set => _inverted = value; }

    public SvRegion ToRegion()
    {
        return new SvRegion
        {
            Chromosome = Chromosome,
            Start = Start.Value,
            End = End.Value,
            OtherChromosome = OtherChromosome,
            OtherStart = OtherStart.Value,
            OtherEnd = OtherEnd.Value,
            Type = Type,
            Inverted = Inverted
        };
    }

    public static Sv FromRegion(SvRegion region)
    {
        return new Sv
        {
            Chromosome = region.Chromosome,
            Start = region.Start,
            End = region.End,
            OtherChromosome = region.OtherChromosome,
            OtherStart = region.OtherStart,
            OtherEnd = region.OtherEnd,
            Type = region.Type,
            Inverted = region.Inverted
        };
    }
}
