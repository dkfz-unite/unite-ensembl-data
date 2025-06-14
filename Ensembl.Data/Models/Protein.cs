﻿namespace Ensembl.Data.Models;

public record Protein
{
    public string Id { get; set; }
    public string TranscriptId { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int Length { get; set; }
    public bool IsCanonical { get; set; }

    public ProteinFeature[] Features { get; set; }


    public Protein(Entities.Translation entity)
    {
        Id = entity.StableId;

        TranscriptId = entity.Transcript?.StableId;

        IsCanonical = entity.Transcript?.CanonicalTranslationId == entity.TranslationId;

        Start = entity.StartExon.SeqRegionStrand == 1
            ? entity.StartExon != null ? entity.StartExon.SeqRegionStart + entity.SeqStart - 1 : 0
            : entity.EndExon != null ? entity.EndExon.SeqRegionEnd - entity.SeqEnd + 1 : 0;

        End = entity.EndExon.SeqRegionStrand == 1
            ? entity.EndExon != null ? entity.EndExon.SeqRegionStart + entity.SeqEnd - 1 : 0
            : entity.StartExon != null ? entity.StartExon.SeqRegionEnd - entity.SeqStart + 1 : 0;
    }
}
