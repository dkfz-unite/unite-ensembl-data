using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("seq_region_attrib")]
public partial class SeqRegionAttrib
{
    [Column("seq_region_id")]
    [Key]
    public int SeqRegionId { get; set; }

    [Column("attrib_type_id")]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }
}
