using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class Translation
    {
        public int TranslationId { get; set; }
        public int TranscriptId { get; set; }
        public int SeqStart { get; set; }
        public int StartExonId { get; set; }
        public int SeqEnd { get; set; }
        public int EndExonId { get; set; }
        public string StableId { get; set; }
        public short? Version { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Transcript Transcript { get; set; }
        public virtual Exon StartExon { get; set; }
        public virtual Exon EndExon { get; set; }
        public virtual ICollection<ProteinFeature> ProteinFeatures { get; set; }
    }
}
