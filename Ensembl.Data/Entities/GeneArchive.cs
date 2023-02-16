using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class GeneArchive
    {
        public string GeneStableId { get; set; }
        public short GeneVersion { get; set; }
        public string TranscriptStableId { get; set; }
        public short TranscriptVersion { get; set; }
        public string TranslationStableId { get; set; }
        public short TranslationVersion { get; set; }
        public int? PeptideArchiveId { get; set; }
        public int MappingSessionId { get; set; }
    }
}
