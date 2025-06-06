using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("ontology_xref")]
public partial class OntologyXref
{
    [Column("object_xref_id")]
    [Key]
    public int ObjectXrefId { get; set; }

    [Column("source_xref_id")]
    public int? SourceXrefId { get; set; }

    [Column("linked_type")]
    public string LinkageType { get; set; }
}
