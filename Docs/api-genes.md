# Ensembl Genes API

Gene lookups support Ensembl stable IDs and gene symbols. Lookup values are
case-insensitive.

Routes use `{genome}` equal to `grch37` or `grch38`. The same routes without the
`/{genome}` segment are direct aliases for GRCh37.

## Query parameters

| Parameter | Applies to | Type | Default | Description |
| --- | --- | --- | --- | --- |
| `length` | Metadata endpoints | Boolean | `false` | Calculate the exonic length of the canonical transcript. |
| `expand` | Metadata endpoints | Boolean | `false` | Include the canonical transcript, its protein and protein features. |

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/{genome}/genes/id/{id}` | Find one gene by Ensembl stable ID. |
| `GET` | `/api/{genome}/genes/id/{id}/sequence` | Get the gene sequence by Ensembl stable ID. |
| `POST` | `/api/{genome}/genes/id` | Find genes by Ensembl stable IDs. |
| `GET` | `/api/{genome}/genes/symbol/{symbol}` | Find one gene by symbol. |
| `GET` | `/api/{genome}/genes/symbol/{symbol}/sequence` | Get the gene sequence by symbol. |
| `POST` | `/api/{genome}/genes/symbol` | Find genes by symbols. |

GET example:

```http
GET /api/grch37/genes/id/ENSG00000146648?length=true&expand=true
```

POST requests contain a JSON array of IDs or symbols. Blank values are ignored
and duplicate values are removed.

```http
POST /api/grch38/genes/id?length=false&expand=false
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
GET /api/grch37/genes/symbol/EGFR/sequence
```

The response is a JSON string containing the complete genomic gene span,
including introns, in feature orientation from 5′ to 3′.

## Responses

| Status | Meaning |
| --- | --- |
| `200` | The request succeeded. |
| `400` | The request or body is invalid. |
| `404` | A single requested gene was not found. |
| `500` | Required annotation or reference data is missing or inconsistent. |
