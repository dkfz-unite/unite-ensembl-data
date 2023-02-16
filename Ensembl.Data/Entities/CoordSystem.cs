using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class CoordSystem
    {
        public int CoordSystemId { get; set; }
        public int SpeciesId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int Rank { get; set; }
    }
}
