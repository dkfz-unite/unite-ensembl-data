using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("translation")]
public partial class Translation
{
    [Column("translation_id")]
    [Key]
    public int TranslationId { get; set; }

    [Column("transcript_id")]
    public int TranscriptId { get; set; }

    [Column("seq_start")]
    public int SeqStart { get; set; }

    [Column("start_exon_id")]
    public int StartExonId { get; set; }

    [Column("seq_end")]
    public int SeqEnd { get; set; }

    [Column("end_exon_id")]
    public int EndExonId { get; set; }

    [Column("stable_id")]
    public string StableId { get; set; }

    [Column("version")]
    public short? Version { get; set; }

    [Column("created_date")]
    public DateTime? CreatedDate { get; set; }

    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }


    [ForeignKey(nameof(TranscriptId))]
    public virtual Transcript Transcript { get; set; }

    [ForeignKey(nameof(StartExonId))]
    public virtual Exon StartExon { get; set; }

    [ForeignKey(nameof(EndExonId))]
    public virtual Exon EndExon { get; set; }

    [InverseProperty(nameof(ProteinFeature.Translation))]
    public virtual ICollection<ProteinFeature> ProteinFeatures { get; set; }
}
