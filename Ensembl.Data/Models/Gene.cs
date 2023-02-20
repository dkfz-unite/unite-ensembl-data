using System;

namespace Ensembl.Data.Models
{
    public record Gene
    {
        public string Request { get; set; }
        public string Id { get; set; }
        public short? Version { get; set; }
        public string Chromosome { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Length { get; set; }
        public int ExonicLength { get; set; }
        public bool Strand { get; set; }
        public string Biotype { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }

        public Transcript Transcript { get; set; }


        public Gene(Entities.Gene entity)
        {
            Id = entity.StableId;
            Version = entity.Version;
            Chromosome = entity.SeqRegion.Name;
            Start = entity.SeqRegionStart;
            End = entity.SeqRegionEnd;
            Length = entity.SeqRegionEnd - entity.SeqRegionStart + 1;
            Strand = entity.SeqRegionStrand == 1 ? true : false;
            Biotype = entity.Biotype;
            Symbol = entity.Xref.DisplayLabel;
            Description = entity.Description;
        }
    }
}

