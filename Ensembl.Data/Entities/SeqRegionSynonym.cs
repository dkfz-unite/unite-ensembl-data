using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class SeqRegionSynonym
    {
        public int SeqRegionSynonymId { get; set; }
        public int SeqRegionId { get; set; }
        public string Synonym { get; set; }
        public int? ExternalDbId { get; set; }

        public virtual SeqRegion SeqRegion { get; set; }
        public virtual ExternalDb ExternalDb { get; set; }
    }
}
