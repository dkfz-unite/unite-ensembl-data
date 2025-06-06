using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("attrib_type")]
public partial class AttribType
{
    [Column("attrib_type_id")]
    [Key]
    public short AttribTypeId { get; set; }

    [Column("code")]
    public string Code { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string Description { get; set; }
}
