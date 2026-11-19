using System.ComponentModel.DataAnnotations.Schema;
using Ensembl.Data.Attributes;

namespace Ensembl.Data.Entities;

[Table("assembly")]
public class AssemblyMapping
{
    [Column("asm_seq_region_id")]
    [CompositeKey("asm_seq_region_id", Order = 1)]
    public int AssemblySeqRegionId { get; set; }

    [Column("cmp_seq_region_id")]
    [CompositeKey("cmp_seq_region_id", Order = 2)]
    public int ComponentSeqRegionId { get; set; }

    [Column("asm_start")]
    [CompositeKey("asm_start", Order = 3)]
    public int AssemblyStart { get; set; }

    [Column("asm_end")]
    [CompositeKey("asm_end", Order = 4)]
    public int AssemblyEnd { get; set; }

    [Column("cmp_start")]
    [CompositeKey("cmp_start", Order = 5)]
    public int ComponentStart { get; set; }

    [Column("cmp_end")]
    [CompositeKey("cmp_end", Order = 6)]
    public int ComponentEnd { get; set; }

    [Column("ori")]
    [CompositeKey("ori", Order = 7)]
    public short Orientation { get; set; }


    [ForeignKey(nameof(AssemblySeqRegionId))]
    public virtual SeqRegion AssemblySeqRegion { get; set; }

    [ForeignKey(nameof(ComponentSeqRegionId))]
    public virtual SeqRegion ComponentSeqRegion { get; set; }
}
