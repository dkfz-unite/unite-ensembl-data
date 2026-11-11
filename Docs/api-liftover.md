# Variant liftover API

The liftover API converts UNITE simple mutations and copy-number variants
between GRCh37 and GRCh38.
Both source and target builds are explicit in the route.
The coordinate conversion and quality-control rules are described in
[Liftover method and quality rules](./liftover.md).

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/grch37/liftover/grch38/sms` | Lift simple mutations from GRCh37 to GRCh38. |
| `POST` | `/api/grch38/liftover/grch37/sms` | Lift simple mutations from GRCh38 to GRCh37. |
| `POST` | `/api/grch37/liftover/grch38/cnvs` | Lift copy-number variants from GRCh37 to GRCh38. |
| `POST` | `/api/grch38/liftover/grch37/cnvs` | Lift copy-number variants from GRCh38 to GRCh37. |

## Simple mutations

The body is an array of mutations in the same representation used by the Omics
feed:

```json
[
  {
    "chromosome": "7",
    "start": 140453136,
    "ref": "A",
    "alt": "T"
  }
]
```

Coordinates are one-based and inclusive. `start` is required. `end` is omitted
for single-position variants; when present, it must be greater than `start`.
For example, a three-base variant uses `start: 140453136` and
`end: 140453138`. `ref` and `alt` are limited to 200 bases. Insertions have a
null `ref`; their `start` is the base after the VCF anchor and `end` is omitted.
Deletions have a null `alt`.

The response preserves input order and pairs every source mutation with its
lifted target. `target` is null when the mutation cannot be lifted:

```json
[
  {
    "source": {
      "chromosome": "7",
      "start": 140453136,
      "end": null,
      "ref": "A",
      "alt": "T"
    },
    "target": {
      "chromosome": "7",
      "start": 140753336,
      "end": null,
      "ref": "A",
      "alt": "T"
    },
    "reason": null
  }
]
```

Expected failure reasons are:

- `coordinate_not_mapped`
- `target_reference_mismatch`
- `invalid_mapping`

The liftover API assumes that submitted variants are valid for the source
genome and does not compare their submitted reference alleles with the source
database. The corresponding source and target genome sequences must still
match for a variant to be lifted. Insertions are converted internally to an
anchored representation using the source base immediately before the insertion
start. Reverse-strand mappings reverse-complement the alleles.

Dedicated methods and endpoints for validating submitted variants against a
reference genome may be added later as an optional quality-control operation.
For now, this validation remains the responsibility of earlier pipeline stages.

## Copy-number variants

The CNV body contains chromosome ranges:

```json
[
  {
    "chromosome": "7",
    "start": 140453136,
    "end": 140454136
  }
]
```

Both coordinates are required, positive, one-based values and `end` must be
greater than `start`. The complete CNV range is mapped with a minimum mapped
source-base ratio of 95%. The target must remain on the same primary chromosome
and its length must be greater than half and less than twice the source length. No
reference-sequence comparison is performed.

The response uses the same source, target and reason structure as simple
mutations. A CNV fails with `coordinate_not_mapped` when the complete range does
not satisfy the mapping threshold, `length_change_exceeded` when the target
length is outside the accepted range, or `invalid_mapping` when the target is
not a valid range on the same supported chromosome.

Missing chain files, database failures and liftover process failures are server
errors rather than per-variant failures.
