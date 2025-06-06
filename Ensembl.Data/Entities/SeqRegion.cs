using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("seq_region")]
public partial class SeqRegion
{
    [Column("seq_region_id")]
    [Key]
    public int SeqRegionId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("coord_system_id")]
    public int CoordSystemId { get; set; }

    [Column("length")]
    public int Length { get; set; }


    [ForeignKey(nameof(CoordSystemId))]
    public virtual CoordSystem CoordSystem { get; set; }

    [InverseProperty(nameof(SeqRegionSynonym.SeqRegion))]
    public virtual ICollection<SeqRegionSynonym> SeqRegionSynonyms { get; set; }
}
