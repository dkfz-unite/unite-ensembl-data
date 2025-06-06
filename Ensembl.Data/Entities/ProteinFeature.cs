using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("protein_feature")]
public partial class ProteinFeature
{
    [Column("protein_feature_id")]
    [Key]
    public int ProteinFeatureId { get; set; }

    [Column("translation_id")]
    public int TranslationId { get; set; }

    [Column("seq_start")]
    public int SeqStart { get; set; }

    [Column("seq_end")]
    public int SeqEnd { get; set; }

    [Column("hit_start")]
    public int HitStart { get; set; }

    [Column("hit_end")]
    public int HitEnd { get; set; }

    [Column("hit_name")]
    public string HitName { get; set; }

    [Column("analysis_id")]
    public short AnalysisId { get; set; }

    [Column("score")]
    public double? Score { get; set; }

    [Column("evalue")]
    public double? Evalue { get; set; }

    [Column("perc_ident")]
    public float? PercIdent { get; set; }

    [Column("external_data")]
    public string ExternalData { get; set; }

    [Column("hit_description")]
    public string HitDescription { get; set; }

    [Column("cigar_line")]
    public string CigarLine { get; set; }

    [Column("align_type")]
    public string AlignType { get; set; }


    [ForeignKey(nameof(TranslationId))]
    public virtual Translation Translation { get; set; }
}
