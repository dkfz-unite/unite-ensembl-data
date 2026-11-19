using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensembl.Data.Entities;

[Table("assembly_exception")]
public class AssemblyException
{
    [Column("assembly_exception_id")]
    [Key]
    public int AssemblyExceptionId { get; set; }

    [Column("seq_region_id")]
    public int SeqRegionId { get; set; }

    [Column("seq_region_start")]
    public int SeqRegionStart { get; set; }

    [Column("seq_region_end")]
    public int SeqRegionEnd { get; set; }

    [Column("exc_type")]
    public string ExceptionType { get; set; }

    [Column("exc_seq_region_id")]
    public int ExceptionSeqRegionId { get; set; }

    [Column("exc_seq_region_start")]
    public int ExceptionSeqRegionStart { get; set; }

    [Column("exc_seq_region_end")]
    public int ExceptionSeqRegionEnd { get; set; }

    [Column("ori")]
    public int Orientation { get; set; }


    [ForeignKey(nameof(SeqRegionId))]
    public virtual SeqRegion SeqRegion { get; set; }

    [ForeignKey(nameof(ExceptionSeqRegionId))]
    public virtual SeqRegion ExceptionSeqRegion { get; set; }
}
