using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ensembl.Data.Attributes;

namespace Ensembl.Data.Entities;

[Table("exon_transcript")]
public partial class ExonTranscript
{
    [Column("exon_id")]
    [CompositeKey("exon_id", Order = 1)]
    public int ExonId { get; set; }

    [Column("transcript_id")]
    [CompositeKey("transcript_id", Order = 2)]
    public int TranscriptId { get; set; }

    [Column("rank")]
    [CompositeKey("rank", Order = 3)]
    public int Rank { get; set; }

    [ForeignKey(nameof(ExonId))]
    public virtual Exon Exon { get; set; }

    [ForeignKey(nameof(TranscriptId))]
    public virtual Transcript Transcript { get; set; }
}
