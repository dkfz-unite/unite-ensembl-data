using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class Transcript
    {
        public int TranscriptId { get; set; }
        public int? GeneId { get; set; }
        public short AnalysisId { get; set; }
        public int SeqRegionId { get; set; }
        public int SeqRegionStart { get; set; }
        public int SeqRegionEnd { get; set; }
        public short SeqRegionStrand { get; set; }
        public int? DisplayXrefId { get; set; }
        public string Source { get; set; }
        public string Biotype { get; set; }
        public string Description { get; set; }
        public bool? IsCurrent { get; set; }
        public int? CanonicalTranslationId { get; set; }
        public string StableId { get; set; }
        public short? Version { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Gene Gene { get; set; }
        public virtual SeqRegion SeqRegion { get; set; }
        public virtual Xref Xref { get; set; }
        public virtual Translation Translation { get; set; }
        public virtual ICollection<ExonTranscript> TranscriptExons { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
