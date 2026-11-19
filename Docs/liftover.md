# Liftover method and quality rules

The service converts genomic coordinates between GRCh37 and GRCh38 with the
UCSC `liftOver` command and the corresponding UCSC chain files. Simple
mutations, copy-number variants and structural variants use different quality
rules because their biological representations have different requirements.

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

## Structural variants

The service treats the stored SV breakpoint ranges as exact cuts, not as
confidence intervals. A cut at position `p` is represented internally by the
two adjacent reference bases `p..p + 1`. Both bases must map together, and the
source sequence must equal the target sequence after accounting for the chain
orientation. The returned cut is normalized to the forward reference strand as
`start..start + 1`.

This two-base representation preserves the location between reference bases.
Mapping only one base would not identify which side of that base contains the
cut. Genuine imprecise breakpoints expressed through caller confidence
intervals are not currently represented by the UNITE SV model.

### Range-based events

`DEL`, `TDUP`, `INS` and `INV` use two cuts on one chromosome. They are
accepted only when all of the following are true:

1. Both cuts map and pass the reference-sequence check.
2. The complete interval between the cuts maps with at least 95% source-base
   coverage.
3. The target interval is on the same primary chromosome, and its length is
   greater than half and less than twice the source length.
4. The separately mapped cuts agree with the ends and orientation of the
   mapped complete interval.

The complete interval is the sequence between the cuts:
`min(start_1, start_2) + 1 .. max(start_1, start_2)`. The independent cut
checks protect base-exact transcript-disrupting boundaries, while the complete
interval check prevents acceptance when only the ends map.

### Translocations and complex events

`ITX` and `CTX` are mapped as two exact cuts. Chromosome arms extending away
from the cuts are not mapped as artificial intervals: the event describes a
new adjacency, and the two reference cuts are the coordinates that can be
projected through an assembly alignment. `ITX` must remain intrachromosomal and
`CTX` must remain interchromosomal after conversion.

`COM` applies the same two-cut operation and preserves whether its cuts are on
the same or different chromosomes. This covers only the two-breakpoint
representation available in the current UNITE model; it does not reconstruct
multi-breakpoint complex rearrangements.

When the assembly-chain orientation changes at exactly one of the two cuts,
the returned `inverted` value is toggled. It is preserved when both cuts have
the same orientation change. A null value remains null because the service
cannot infer sample-event orientation from the reference assemblies.

### Dispersed duplications

`DUP` has two different locations. The first is the copied source interval;
the second is the destination insertion cut. The source interval is checked as
a complete range using the 95% mapped-base and length-ratio rules. Its two
boundary cuts and the destination cut must also pass the exact reference check.
The target source interval must agree with its separately mapped boundaries.

If the assembly orientation of the copied interval differs from that of the
destination cut, a non-null `inverted` value is toggled. This preserves the
relative orientation of the inserted copy in the target assembly.

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
- [GA4GH VCF specification and breakend representation](https://github.com/samtools/hts-specs/blob/master/VCFv4.5.pdf)
