using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("exon")]
public partial class Exon
{
    [Column("exon_id")]
    [Key]
    public int ExonId { get; set; }

    [Column("seq_region_id")]
    public int SeqRegionId { get; set; }

    [Column("seq_region_start")]
    public int SeqRegionStart { get; set; }

    [Column("seq_region_end")]
    public int SeqRegionEnd { get; set; }

    [Column("seq_region_strand")]
    public short SeqRegionStrand { get; set; }

    [Column("phase")]
    public short Phase { get; set; }

    [Column("end_phase")]
    public short EndPhase { get; set; }

    [Column("is_current")]
    public bool IsCurrent { get; set; }

    [Column("is_constitutive")]
    public bool IsConstitutive { get; set; }

    [Column("stable_id")]
    public string StableId { get; set; }

    [Column("version")]
    public short? Version { get; set; }

    [Column("created_date")]
    public DateTime? CreatedDate { get; set; }

    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }


    [ForeignKey(nameof(SeqRegionId))]
    public virtual SeqRegion SeqRegion { get; set; }
}
