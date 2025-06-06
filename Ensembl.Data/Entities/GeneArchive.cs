using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("gene_archive")]
public partial class GeneArchive
{
    [Column("gene_stable_id")]
    [Key]
    public string GeneStableId { get; set; }

    [Column("gene_version")]
    public short GeneVersion { get; set; }

    [Column("transcript_stable_id")]
    public string TranscriptStableId { get; set; }

    [Column("transcript_version")]
    public short TranscriptVersion { get; set; }

    [Column("translation_stable_id")]
    public string TranslationStableId { get; set; }

    [Column("translation_version")]
    public short TranslationVersion { get; set; }

    [Column("peptide_archive_id")]
    public int? PeptideArchiveId { get; set; }

    [Column("mapping_session_id")]
    public int MappingSessionId { get; set; }
}
