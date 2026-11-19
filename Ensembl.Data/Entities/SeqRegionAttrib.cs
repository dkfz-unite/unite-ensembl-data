using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Entities;

[Table("seq_region_attrib")]
[Keyless]
public partial class SeqRegionAttrib
{
    [Column("seq_region_id")]
    public int SeqRegionId { get; set; }

    [Column("attrib_type_id")]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }


    [ForeignKey(nameof(AttribTypeId))]
    public virtual AttribType AttribType { get; set; }
}
