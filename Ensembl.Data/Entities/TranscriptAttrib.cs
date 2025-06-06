using System.ComponentModel.DataAnnotations.Schema;
using Ensembl.Data.Attributes;

namespace Ensembl.Data.Entities;

[Table("transcript_attrib")]
public partial class TranscriptAttrib
{
    [Column("transcript_id")]
    [CompositeKey("transcript_id", Order = 1)]
    public int TranscriptId { get; set; }

    [Column("attrib_type_id")]
    [CompositeKey("attrib_type_id", Order = 2)]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }
}
