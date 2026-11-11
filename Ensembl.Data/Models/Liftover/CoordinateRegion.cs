namespace Ensembl.Data.Models.Liftover;

public class CoordinateRegion
{
    public int Id { get; set; }

    public string Chromosome { get; set; }

    public int Start { get; set; }

    public int End { get; set; }

    public bool? Forward { get; set; }
}
