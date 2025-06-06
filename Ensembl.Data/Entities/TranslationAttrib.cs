using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("translation_attrib")]
public partial class TranslationAttrib
{
    [Column("translation_id")]
    [Key]
    public int TranslationId { get; set; }

    [Column("attrib_type_id")]
    public short AttribTypeId { get; set; }

    [Column("value")]
    public string Value { get; set; }
}
