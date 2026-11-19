using System.Text.Json.Serialization;
using Unite.Essentials.Attributes;

namespace Ensembl.Data.Models.Liftover;

[JsonConverter(typeof(EnumAliasJsonConverter<Chromosome>))]
public enum Chromosome
{
    /// <summary>
    /// Chromosome 1.
    /// </summary>
    [EnumAlias("1", "chr1")]
    Chr1 = 1,

    /// <summary>
    /// Chromosome 2.
    /// </summary>
    [EnumAlias("2", "chr2")]
    Chr2 = 2,

    /// <summary>
    /// Chromosome 3.
    /// </summary>
    [EnumAlias("3", "chr3")]
    Chr3 = 3,

    /// <summary>
    /// Chromosome 4.
    /// </summary>
    [EnumAlias("4", "chr4")]
    Chr4 = 4,

    /// <summary>
    /// Chromosome 5.
    /// </summary>
    [EnumAlias("5", "chr5")]
    Chr5 = 5,

    /// <summary>
    /// Chromosome 6.
    /// </summary>
    [EnumAlias("6", "chr6")]
    Chr6 = 6,

    /// <summary>
    /// Chromosome 7.
    /// </summary>
    [EnumAlias("7", "chr7")]
    Chr7 = 7,

    /// <summary>
    /// Chromosome 8.
    /// </summary>
    [EnumAlias("8", "chr8")]
    Chr8 = 8,

    /// <summary>
    /// Chromosome 9.
    /// </summary>
    [EnumAlias("9", "chr9")]
    Chr9 = 9,

    /// <summary>
    /// Chromosome 10.
    /// </summary>
    [EnumAlias("10", "chr10")]
    Chr10 = 10,

    /// <summary>
    /// Chromosome 11.
    /// </summary>
    [EnumAlias("11", "chr11")]
    Chr11 = 11,

    /// <summary>
    /// Chromosome 12.
    /// </summary>
    [EnumAlias("12", "chr12")]
    Chr12 = 12,

    /// <summary>
    /// Chromosome 13.
    /// </summary>
    [EnumAlias("13", "chr13")]
    Chr13 = 13,

    /// <summary>
    /// Chromosome 14.
    /// </summary>
    [EnumAlias("14", "chr14")]
    Chr14 = 14,

    /// <summary>
    /// Chromosome 15.
    /// </summary>
    [EnumAlias("15", "chr15")]
    Chr15 = 15,

    /// <summary>
    /// Chromosome 16.
    /// </summary>
    [EnumAlias("16", "chr16")]
    Chr16 = 16,

    /// <summary>
    /// Chromosome 17.
    /// </summary>
    [EnumAlias("17", "chr17")]
    Chr17 = 17,

    /// <summary>
    /// Chromosome 18.
    /// </summary>
    [EnumAlias("18", "chr18")]
    Chr18 = 18,

    /// <summary>
    /// Chromosome 19.
    /// </summary>
    [EnumAlias("19", "chr19")]
    Chr19 = 19,

    /// <summary>
    /// Chromosome 20.
    /// </summary>
    [EnumAlias("20", "chr20")]
    Chr20 = 20,

    /// <summary>
    /// Chromosome 21.
    /// </summary>
    [EnumAlias("21", "chr21")]
    Chr21 = 21,

    /// <summary>
    /// Chromosome 22.
    /// </summary>
    [EnumAlias("22", "chr22")]
    Chr22 = 22,

    /// <summary>
    /// Chromosome X.
    /// </summary>
    [EnumAlias("X", "chrX")]
    ChrX = 23,

    /// <summary>
    /// Chromosome Y.
    /// </summary>
    [EnumAlias("Y", "chrY")]
    ChrY = 24,

    /// <summary>
    /// Mitochondrial chromosome.
    /// </summary>
    [EnumAlias("MT", "M", "chrM", "chrMT")]
    ChrMT = 25
}
