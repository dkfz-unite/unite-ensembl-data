using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("interpro")]
public partial class Interpro
{
    [Column("interpro_ac")]
    public string InterproAc { get; set; }

    [Column("id")]
    [Key]
    public string Id { get; set; }
}
