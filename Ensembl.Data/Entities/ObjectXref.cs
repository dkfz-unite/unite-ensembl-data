using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class ObjectXref
    {
        public int ObjectXrefId { get; set; }
        public int EnsemblId { get; set; }
        public string EnsemblObjectType { get; set; }
        public int XrefId { get; set; }
        public string LinkageAnnotation { get; set; }
        public short? AnalysisId { get; set; }
    }
}
