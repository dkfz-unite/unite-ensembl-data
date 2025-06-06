namespace Ensembl.Data.Models;

public record ProteinFeature
{
    public int SeqStart { get; set; }
    public int SeqEnd { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double? Evalue { get; set; }


    public ProteinFeature(Entities.ProteinFeature entity)
    {
        SeqStart = entity.SeqStart;
        SeqEnd = entity.SeqEnd;
        Start = entity.HitStart;
        End = entity.HitEnd;
        Name = entity.HitName;
        Description = entity.HitDescription;
        Evalue = entity.Evalue;
    }
}
