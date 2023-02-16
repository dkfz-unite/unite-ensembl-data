using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class ProteinFeature
    {
        public int ProteinFeatureId { get; set; }
        public int TranslationId { get; set; }
        public int SeqStart { get; set; }
        public int SeqEnd { get; set; }
        public int HitStart { get; set; }
        public int HitEnd { get; set; }
        public string HitName { get; set; }
        public short AnalysisId { get; set; }
        public double? Score { get; set; }
        public double? Evalue { get; set; }
        public float? PercIdent { get; set; }
        public string ExternalData { get; set; }
        public string HitDescription { get; set; }
        public string CigarLine { get; set; }
        public string AlignType { get; set; }

        public virtual Translation Translation { get; set; }
    }
}
