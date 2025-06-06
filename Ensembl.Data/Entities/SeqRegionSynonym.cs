using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("seq_region_synonym")]
public partial class SeqRegionSynonym
{
    [Column("seq_region_synonym_id")]
    [Key]
    public int SeqRegionSynonymId { get; set; }

    [Column("seq_region_id")]
    public int SeqRegionId { get; set; }

    [Column("synonym")]
    public string Synonym { get; set; }

    [Column("external_db_id")]
    public int? ExternalDbId { get; set; }


    [ForeignKey(nameof(SeqRegionId))]
    public virtual SeqRegion SeqRegion { get; set; }

    [ForeignKey(nameof(ExternalDbId))]
    public virtual ExternalDb ExternalDb { get; set; }
}
