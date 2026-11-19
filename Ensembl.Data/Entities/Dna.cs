using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("dna")]
public class Dna
{
    [Column("seq_region_id")]
    [Key]
    public int SeqRegionId { get; set; }

    [Column("sequence")]
    public string Sequence { get; set; }


    [ForeignKey(nameof(SeqRegionId))]
    public virtual SeqRegion SeqRegion { get; set; }
}
