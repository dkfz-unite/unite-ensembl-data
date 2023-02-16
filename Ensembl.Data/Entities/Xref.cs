using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class Xref
    {
        public int XrefId { get; set; }
        public int ExternalDbId { get; set; }
        public string DbprimaryAcc { get; set; }
        public string DisplayLabel { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string InfoType { get; set; }
        public string InfoText { get; set; }

        public virtual ExternalDb ExternalDb { get; set; }
    }
}
