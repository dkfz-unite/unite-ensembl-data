# Ensembl Genes API

Gene lookups support Ensembl stable IDs and gene symbols. Lookup values are
case-insensitive.

## Query parameters

| Parameter | Applies to | Type | Default | Description |
| --- | --- | --- | --- | --- |
| `grch` | All endpoints | Integer | `37` | Reference genome version. Supported values are `37` and `38`. |
| `length` | Metadata endpoints | Boolean | `false` | Calculate the exonic length of the canonical transcript. |
| `expand` | Metadata endpoints | Boolean | `false` | Include the canonical transcript, its protein and protein features. |

The database for the selected `grch` version must be installed and accessible.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/genes/id/{id}` | Find one gene by Ensembl stable ID. |
| `GET` | `/api/genes/id/{id}/sequence` | Get the gene sequence by Ensembl stable ID. |
| `POST` | `/api/genes/id` | Find genes by Ensembl stable IDs. |
| `GET` | `/api/genes/symbol/{symbol}` | Find one gene by symbol. |
| `GET` | `/api/genes/symbol/{symbol}/sequence` | Get the gene sequence by symbol. |
| `POST` | `/api/genes/symbol` | Find genes by symbols. |

GET example:

```http
GET /api/genes/id/ENSG00000146648?length=true&expand=true&grch=37
```

POST requests contain a JSON array of IDs or symbols. Blank values are ignored
and duplicate values are removed.

```http
POST /api/genes/id?length=false&expand=false&grch=37
Content-Type: application/json
```

```json
[
  "ENSG00000146648",
  "ENSG00000155657"
]
```

Single GET requests return a gene JSON object or `404` when no gene matches.
POST requests return a JSON array, which can be empty when nothing matches.

## Sequence responses

Example:

```http
GET /api/genes/symbol/EGFR/sequence?grch=37
```

The response is a JSON string containing the complete genomic gene span,
including introns, in feature orientation from 5′ to 3′.

## Responses

| Status | Meaning |
| --- | --- |
| `200` | The request succeeded. |
| `400` | The request is invalid, the body is missing, or `grch` is unsupported or unavailable. |
| `404` | A single requested gene was not found. |
| `500` | Required annotation or reference data is missing or inconsistent. |
