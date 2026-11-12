namespace Ensembl.Data.Models.Liftover;

public class CoordinateRegion
{
    public int Id { get; set; }

    public string Chromosome { get; set; }

    public int Start { get; set; }

    public int End { get; set; }

    /// <summary>
    /// Region strand: true for forward strand, false for reverse strand, null if unknown or unset.
    /// </summary>
    public bool? Strand { get; set; }
}
