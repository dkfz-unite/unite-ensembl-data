# Sequence size and retrieval measurement

Measured against the installed Ensembl release 116 GRCh37 annotation and
reference database. The HTTP timing was repeated on 2026-11-09 with the Release
build of the feature sequence API.

## Largest installed gene

Genes were ordered by inclusive genomic span (`end - start + 1`), restricted
to the GRCh37 chromosome coordinate system and chromosomes 1–22, X, Y and MT.

- Gene: **CNTNAP2**, `ENSG00000174469`.
- Chromosome: **7**, forward strand.
- Coordinates: **145,813,453–148,118,090**, 1-based and inclusive.
- Genomic length: **2,304,638 bases**.
- Retrieved length: **2,304,638 characters**, with no unknown bases.

GRCh38 gene annotations were not yet installed when this query was run, so this
is the largest gene in the installed GRCh37 data, not a comparison across builds.

## Size

KB and MB below are decimal (1,000 and 1,000,000 bytes); MiB is binary
(1,048,576 bytes). ASCII nucleotide letters occupy one UTF-8 byte each. .NET
strings use two bytes per UTF-16 character; the character-storage figure
excludes the small string object/header/alignment overhead.

| Representation | Bytes | KB | MB | MiB |
| --- | ---: | ---: | ---: | ---: |
| UTF-8 sequence | 2,304,638 | 2,304.638 | 2.304638 | 2.197874 |
| .NET UTF-16 character storage | 4,609,276 | 4,609.276 | 4.609276 | 4.395748 |
| Serialized sequence (UTF-8 JSON) | 2,304,640 | 2,304.640 | 2.304640 | 2.197876 |

## HTTP timing

The Release service and MySQL database ran locally. Measurements include
database access, sequence assembly, JSON serialization and HTTP transfer over
localhost. The endpoint was called once and then seven additional times without
clearing database or operating-system caches.

| First request | Warm median | Warm minimum | Warm maximum |
| ---: | ---: | ---: | ---: |
| 55.1 ms | 55.9 ms | 52.2 ms | 60.6 ms |

No feature-sequence response limit is applied. The largest installed GRCh37
gene can be returned in well under one second on the local setup.
