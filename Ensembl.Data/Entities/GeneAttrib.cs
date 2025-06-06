using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("gene_attrib")]
public partial class GeneAttrib
{
    [Column("gene_id")]
    [Key]
    public int GeneId { get; set; }

    [Column("attrib_type_id")]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }
}
