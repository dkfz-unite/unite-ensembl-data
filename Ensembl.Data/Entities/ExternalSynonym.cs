using System.ComponentModel.DataAnnotations.Schema;
using Ensembl.Data.Attributes;

namespace Ensembl.Data.Entities;

[Table("external_synonym")]
public partial class ExternalSynonym
{
    [Column("xref_id")]
    [CompositeKey("xref_id", Order = 1)]
    public int XrefId { get; set; }

    [Column("synonym")]
    [CompositeKey("synonym", Order = 2)]
    public string Synonym { get; set; }

    [ForeignKey(nameof(XrefId))]
    public virtual Xref Xref { get; set; }
}
