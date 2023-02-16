using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class Biotype
    {
        public int BiotypeId { get; set; }
        public string Name { get; set; }
        public string ObjectType { get; set; }
        public int? AttribTypeId { get; set; }
        public string Description { get; set; }
        public string BiotypeGroup { get; set; }
        public string SoAcc { get; set; }
        public string SoTerm { get; set; }
    }
}
