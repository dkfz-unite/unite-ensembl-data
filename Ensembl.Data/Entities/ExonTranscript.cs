using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class ExonTranscript
    {
        public int ExonId { get; set; }
        public int TranscriptId { get; set; }
        public int Rank { get; set; }

        public virtual Exon Exon { get; set; }
        public virtual Transcript Transcript { get; set; }
    }
}
