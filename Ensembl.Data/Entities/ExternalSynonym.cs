using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class ExternalSynonym
    {
        public int XrefId { get; set; }
        public string Synonym { get; set; }

        public virtual Xref Xref { get; set; }
    }
}
