# Liftover method and quality rules

The service converts genomic coordinates between GRCh37 and GRCh38 with the
UCSC `liftOver` command and the corresponding UCSC chain files. Simple
mutations and copy-number variants use different quality rules because their
biological representations have different requirements.

## Simple mutations

Simple mutations are mapped with all source bases required to map. The service
then retrieves the source and target reference sequences from the installed
Ensembl databases and accepts the mapping only when they match. Reverse-strand
mappings reverse-complement the reference and alternate alleles.

Insertions use the source base immediately before the insertion as an internal
anchor. The anchor is removed again when the target mutation is returned.

## Copy-number variants

Copy-number calls represent continuous genomic segments whose boundaries may
be estimated rather than base-exact. CNVs are therefore mapped as complete
intervals rather than as two independent endpoints.

The following rules are applied:

1. At least 95% of the source interval bases must map to one target interval.
2. The target must be a valid primary chromosome and must be the same
   chromosome as the source.
3. The target-to-source length ratio must be greater than 0.5 and less than 2.0.

The first rule uses the UCSC `-minMatch=0.95` option. It validates aligned
source-base coverage but does not constrain how much sequence may occur between
the mapped target bases. The separate length-ratio rule detects large target
expansions or contractions.

The 95% threshold is fixed rather than reduced for longer CNVs. A percentage
already permits a larger absolute amount of unmapped sequence as an interval
grows, and no validated length-dependent threshold schedule was identified for
GRCh37-to-GRCh38 CNV conversion.

The length-ratio rule follows the default quality-control criterion from
`segment_liftover`, a tool developed for copy-number segments. Endpoint-only
mapping is not used because it can accept intervals whose boundaries map while
most of the interval between them does not.

## Limitations

Liftover projects coordinates through an assembly alignment. It does not
reanalyze the original sample reads and cannot guarantee equivalent sequence in
regions substantially changed between assemblies. Recalling variants against
the target assembly remains preferable when the original sequencing data are
available.

Expected per-variant mapping failures are returned in the response. Missing
chain files, process failures and reference-database failures are service
errors.

## References

- [UCSC liftOver source and command-line options](https://github.com/ucscGenomeBrowser/kent/blob/master/src/hg/liftOver/liftOver.c)
- [UCSC liftOver error explanations](https://www.genome.ucsc.edu/FAQ/FAQdisplay.html)
- [Gao et al. — segment_liftover: a Python tool to convert segments between genome assemblies](https://pmc.ncbi.nlm.nih.gov/articles/PMC5998006/)
- [Luu et al. — Benchmark study comparing liftover tools for genome conversion of epigenome sequencing data](https://pmc.ncbi.nlm.nih.gov/articles/PMC7671393/)
- [GDC Copy Number Variation Analysis Pipeline](https://docs.gdc.cancer.gov/Data/Bioinformatics_Pipelines/CNV_Pipeline/)
