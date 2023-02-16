using System;
using System.Collections.Generic;

#nullable disable

namespace Ensembl.Data.Entities
{
    public partial class TranslationAttrib
    {
        public int TranslationId { get; set; }
        public short AttribTypeId { get; set; }
        public string Value { get; set; }
    }
}
