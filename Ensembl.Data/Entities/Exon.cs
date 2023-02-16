using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class Exon
    {
        public int ExonId { get; set; }
        public int SeqRegionId { get; set; }
        public int SeqRegionStart { get; set; }
        public int SeqRegionEnd { get; set; }
        public short SeqRegionStrand { get; set; }
        public byte Phase { get; set; }
        public byte EndPhase { get; set; }
        public bool? IsCurrent { get; set; }
        public bool IsConstitutive { get; set; }
        public string StableId { get; set; }
        public short? Version { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual SeqRegion SeqRegion { get; set; }
    }
}
