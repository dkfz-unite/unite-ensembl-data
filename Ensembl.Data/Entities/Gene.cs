using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("gene")]
public partial class Gene
{
    [Column("gene_id")]
    [Key]
    public int GeneId { get; set; }

    [Column("biotype")]
    public string Biotype { get; set; }

    [Column("analysis_id")]
    public short AnalysisId { get; set; }

    [Column("seq_region_id")]
    public int SeqRegionId { get; set; }

    [Column("seq_region_start")]
    public int SeqRegionStart { get; set; }

    [Column("seq_region_end")]
    public int SeqRegionEnd { get; set; }

    [Column("seq_region_strand")]
    public short SeqRegionStrand { get; set; }

    [Column("display_xref_id")]
    public int? DisplayXrefId { get; set; }

    [Column("source")]
    public string Source { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("is_current")]
    public bool? IsCurrent { get; set; }

    [Column("canonical_transcript_id")]
    public int CanonicalTranscriptId { get; set; }

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

    [ForeignKey(nameof(DisplayXrefId))]
    public virtual Xref Xref { get; set; }

    [ForeignKey(nameof(CanonicalTranscriptId))]
    public virtual Transcript Transcript { get; set; }

    [InverseProperty(nameof(Transcript.Gene))]
    public virtual ICollection<Transcript> Transcripts { get; set; }
}
