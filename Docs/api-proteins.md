# Ensembl Proteins API

Protein lookups support Ensembl stable IDs, external accessions and protein
symbols. Lookup values are case-insensitive.

Routes use `{genome}` equal to `grch37` or `grch38`. The same routes without the
`/{genome}` segment are direct aliases for GRCh37.

## Query parameters

| Parameter | Applies to | Type | Default | Description |
| --- | --- | --- | --- | --- |
| `expand` | Metadata endpoints | Boolean | `false` | Include protein features such as PFAM domains. |

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/{genome}/proteins/id/{id}` | Find one protein by Ensembl stable ID. |
| `GET` | `/api/{genome}/proteins/id/{id}/sequence` | Get the amino acid sequence by Ensembl protein ID. |
| `POST` | `/api/{genome}/proteins/id` | Find proteins by Ensembl stable IDs. |
| `GET` | `/api/{genome}/proteins/acc/{accession}` | Find one protein by external accession. |
| `GET` | `/api/{genome}/proteins/acc/{accession}/sequence` | Get the amino acid sequence by external accession. |
| `POST` | `/api/{genome}/proteins/acc` | Find proteins by external accessions. |
| `GET` | `/api/{genome}/proteins/symbol/{symbol}` | Find one protein by symbol. |
| `GET` | `/api/{genome}/proteins/symbol/{symbol}/sequence` | Get the amino acid sequence by protein symbol. |
| `POST` | `/api/{genome}/proteins/symbol` | Find proteins by symbols. |

GET examples:

```http
GET /api/grch37/proteins/id/ENSP00000275493?expand=true
GET /api/grch37/proteins/acc/P00533?expand=false
GET /api/grch38/proteins/symbol/EGFR_HUMAN?expand=false
```

POST requests contain a JSON array of IDs, accessions or symbols. Blank values
are ignored and duplicate values are removed.

```http
POST /api/grch38/proteins/acc?expand=false
Content-Type: application/json
```

```json
[
  "P00533",
  "Q9Y6R7"
]
```

Single GET requests return a protein JSON object or `404` when no protein
matches. POST requests return a JSON array, which can be empty when nothing
matches.

## Sequence responses

Example:

```http
GET /api/grch37/proteins/acc/P00533/sequence
```

The response is an amino acid sequence as a JSON string. Translation uses the
annotated coding boundaries, the applicable standard or vertebrate
mitochondrial codon table, and recorded sequence edits. A terminal stop codon
is omitted.

## Responses

| Status | Meaning |
| --- | --- |
| `200` | The request succeeded. |
| `400` | The request or body is invalid. |
| `404` | A single requested protein was not found. |
| `500` | Required annotation or reference data is missing or inconsistent. |
