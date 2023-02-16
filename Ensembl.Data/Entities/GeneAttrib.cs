using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class GeneAttrib
    {
        public int GeneId { get; set; }
        public short AttribTypeId { get; set; }
        public string Value { get; set; }
    }
}
