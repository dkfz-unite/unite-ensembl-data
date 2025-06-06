using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("xref")]
public partial class Xref
{
    [Column("xref_id")]
    [Key]
    public int XrefId { get; set; }

    [Column("external_db_id")]
    public int ExternalDbId { get; set; }

    [Column("dbprimary_acc")]
    public string DbprimaryAcc { get; set; }

    [Column("display_label")]
    public string DisplayLabel { get; set; }

    [Column("version")]
    public string Version { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("info_type")]
    public string InfoType { get; set; }

    [Column("info_text")]
    public string InfoText { get; set; }


    [ForeignKey(nameof(ExternalDbId))]
    public virtual ExternalDb ExternalDb { get; set; }
}
