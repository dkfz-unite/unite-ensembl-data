using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("external_db")]
public partial class ExternalDb
{
    [Column("external_db_id")]
    [Key]
    public int ExternalDbId { get; set; }

    [Column("db_name")]
    public string DbName { get; set; }

    [Column("db_release")]
    public string DbRelease { get; set; }

    [Column("db_status")]
    public string Status { get; set; }

    [Column("priority")]
    public int Priority { get; set; }

    [Column("db_display_name")]
    public string DbDisplayName { get; set; }

    [Column("db_type")]
    public string Type { get; set; }

    [Column("secondary_db_name")]
    public string SecondaryDbName { get; set; }

    [Column("secondary_db_table")]
    public string SecondaryDbTable { get; set; }

    [Column("description")]
    public string Description { get; set; }
}
