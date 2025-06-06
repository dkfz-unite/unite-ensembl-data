using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("object_xref")]
public partial class ObjectXref
{
    [Column("object_xref_id")]
    [Key]
    public int ObjectXrefId { get; set; }

    [Column("ensembl_id")]
    public int EnsemblId { get; set; }

    [Column("ensembl_object_type")]
    public string EnsemblObjectType { get; set; }

    [Column("xref_id")]
    public int XrefId { get; set; }

    [Column("linkage_annotation")]
    public string LinkageAnnotation { get; set; }

    [Column("analysis_id")]
    public short? AnalysisId { get; set; }
}
