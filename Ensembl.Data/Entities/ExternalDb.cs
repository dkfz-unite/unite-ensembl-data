using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class ExternalDb
    {
        public int ExternalDbId { get; set; }
        public string DbName { get; set; }
        public string DbRelease { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public string DbDisplayName { get; set; }
        public string Type { get; set; }
        public string SecondaryDbName { get; set; }
        public string SecondaryDbTable { get; set; }
        public string Description { get; set; }
    }
}
