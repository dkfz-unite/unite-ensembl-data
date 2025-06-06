using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("coord_system")]
public partial class CoordSystem
{
    [Column("coord_system_id")]
    [Key]
    public int CoordSystemId { get; set; }

    [Column("species_id")]
    public int SpeciesId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("version")]
    public string Version { get; set; }

    [Column("rank")]
    public int Rank { get; set; }
}
