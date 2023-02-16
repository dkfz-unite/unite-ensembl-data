using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class OntologyXref
    {
        public int ObjectXrefId { get; set; }
        public int? SourceXrefId { get; set; }
        public string LinkageType { get; set; }
    }
}
