# Ensembl Proteins API

Protein lookups support Ensembl stable IDs, external accessions and protein
symbols. Lookup values are case-insensitive.

## Query parameters

| Parameter | Applies to | Type | Default | Description |
| --- | --- | --- | --- | --- |
| `grch` | All endpoints | Integer | `37` | Reference genome version. Supported values are `37` and `38`. |
| `expand` | Metadata endpoints | Boolean | `false` | Include protein features such as PFAM domains. |

The database for the selected `grch` version must be installed and accessible.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/proteins/id/{id}` | Find one protein by Ensembl stable ID. |
| `GET` | `/api/proteins/id/{id}/sequence` | Get the amino acid sequence by Ensembl protein ID. |
| `POST` | `/api/proteins/id` | Find proteins by Ensembl stable IDs. |
| `GET` | `/api/proteins/acc/{accession}` | Find one protein by external accession. |
| `GET` | `/api/proteins/acc/{accession}/sequence` | Get the amino acid sequence by external accession. |
| `POST` | `/api/proteins/acc` | Find proteins by external accessions. |
| `GET` | `/api/proteins/symbol/{symbol}` | Find one protein by symbol. |
| `GET` | `/api/proteins/symbol/{symbol}/sequence` | Get the amino acid sequence by protein symbol. |
| `POST` | `/api/proteins/symbol` | Find proteins by symbols. |

GET examples:

```http
GET /api/proteins/id/ENSP00000275493?expand=true&grch=37
GET /api/proteins/acc/P00533?expand=false&grch=37
GET /api/proteins/symbol/EGFR_HUMAN?expand=false&grch=37
```

POST requests contain a JSON array of IDs, accessions or symbols. Blank values
are ignored and duplicate values are removed.

```http
POST /api/proteins/acc?expand=false&grch=37
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
GET /api/proteins/acc/P00533/sequence?grch=37
```

The response is an amino acid sequence as a JSON string. Translation uses the
annotated coding boundaries, the applicable standard or vertebrate
mitochondrial codon table, and recorded sequence edits. A terminal stop codon
is omitted.

## Responses

| Status | Meaning |
| --- | --- |
| `200` | The request succeeded. |
| `400` | The request is invalid, the body is missing, or `grch` is unsupported or unavailable. |
| `404` | A single requested protein was not found. |
| `500` | Required annotation or reference data is missing or inconsistent. |
