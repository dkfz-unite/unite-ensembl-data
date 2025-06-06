using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("biotype")]
public partial class Biotype
{
    [Column("biotype_id")]
    [Key]
    public int BiotypeId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("object_type")]
    public string ObjectType { get; set; }

    [Column("attrib_type_id")]
    public int? AttribTypeId { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("biotype_group")]
    public string BiotypeGroup { get; set; }

    [Column("so_acc")]
    public string SoAcc { get; set; }

    [Column("so_term")]
    public string SoTerm { get; set; }
}
