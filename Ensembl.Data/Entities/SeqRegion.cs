using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class SeqRegion
    {
        public int SeqRegionId { get; set; }
        public string Name { get; set; }
        public int CoordSystemId { get; set; }
        public int Length { get; set; }

        public virtual CoordSystem CoordSystem { get; set; }
        public virtual ICollection<SeqRegionSynonym> SeqRegionSynonyms { get; set; }
    }
}
