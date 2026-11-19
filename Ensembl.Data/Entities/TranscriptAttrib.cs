using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ensembl.Data.Entities;

[Table("transcript_attrib")]
[Keyless]
public partial class TranscriptAttrib
{
    [Column("transcript_id")]
    public int TranscriptId { get; set; }

    [Column("attrib_type_id")]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }


    [ForeignKey(nameof(AttribTypeId))]
    public virtual AttribType AttribType { get; set; }
}
