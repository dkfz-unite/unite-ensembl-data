using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("transcript")]
public partial class Transcript
{
    [Column("transcript_id")]
    [Key]
    public int TranscriptId { get; set; }

    [Column("gene_id")]
    public int? GeneId { get; set; }

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

    [Column("biotype")]
    public string Biotype { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("is_current")]
    public bool? IsCurrent { get; set; }

    [Column("canonical_translation_id")]
    public int? CanonicalTranslationId { get; set; }

    [Column("stable_id")]
    public string StableId { get; set; }

    [Column("version")]
    public short? Version { get; set; }

    [Column("created_date")]
    public DateTime? CreatedDate { get; set; }

    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }


    [ForeignKey(nameof(GeneId))]
    public virtual Gene Gene { get; set; }

    [ForeignKey(nameof(SeqRegionId))]
    public virtual SeqRegion SeqRegion { get; set; }

    [ForeignKey(nameof(DisplayXrefId))]
    public virtual Xref Xref { get; set; }

    [ForeignKey(nameof(CanonicalTranslationId))]
    public virtual Translation Translation { get; set; }

    [InverseProperty(nameof(ExonTranscript.Transcript))]
    public virtual ICollection<ExonTranscript> TranscriptExons { get; set; }

    [InverseProperty(nameof(Translation.Transcript))]
    public virtual ICollection<Translation> Translations { get; set; }
}
