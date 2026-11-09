# Ensembl Transcripts API

Transcript lookups support Ensembl stable IDs and transcript symbols. Lookup
values are case-insensitive.

Routes use `{genome}` equal to `grch37` or `grch38`. The same routes without the
`/{genome}` segment are direct aliases for GRCh37.

## Query parameters

| Parameter | Applies to | Type | Default | Description |
| --- | --- | --- | --- | --- |
| `length` | Metadata endpoints | Boolean | `false` | Calculate the total exonic length. |
| `expand` | Metadata endpoints | Boolean | `false` | Include the transcript protein and its protein features. |
| `type` | Sequence endpoints | String | `genomic` | Sequence type: `genomic`, `cdna` or `cds`. |

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/{genome}/transcripts/id/{id}` | Find one transcript by Ensembl stable ID. |
| `GET` | `/api/{genome}/transcripts/id/{id}/sequence` | Get the transcript sequence by Ensembl stable ID. |
| `POST` | `/api/{genome}/transcripts/id` | Find transcripts by Ensembl stable IDs. |
| `GET` | `/api/{genome}/transcripts/symbol/{symbol}` | Find one transcript by symbol. |
| `GET` | `/api/{genome}/transcripts/symbol/{symbol}/sequence` | Get the transcript sequence by symbol. |
| `POST` | `/api/{genome}/transcripts/symbol` | Find transcripts by symbols. |

GET example:

```http
GET /api/grch37/transcripts/id/ENST00000275493?length=true&expand=true
```

POST requests contain a JSON array of IDs or symbols. Blank values are ignored
and duplicate values are removed.

```http
POST /api/grch38/transcripts/symbol?length=false&expand=false
Content-Type: application/json
```

```json
[
  "EGFR-001",
  "TTN-001"
]
```

Single GET requests return a transcript JSON object or `404` when no transcript
matches. POST requests return a JSON array, which can be empty when nothing
matches.

## Sequence responses

Examples:

```http
GET /api/grch37/transcripts/id/ENST00000275493/sequence?type=genomic
GET /api/grch37/transcripts/id/ENST00000275493/sequence?type=cdna
GET /api/grch37/transcripts/id/ENST00000275493/sequence?type=cds
```

Sequence types:

- `genomic` returns the complete genomic transcript span in feature orientation
  from 5′ to 3′.
- `cdna` returns the spliced exon sequence, including untranslated regions.
- `cds` returns the canonical translation coding sequence. A noncoding
  transcript returns JSON `null`.

DNA sequences are returned as JSON strings.

## Responses

| Status | Meaning |
| --- | --- |
| `200` | The request succeeded. |
| `400` | The request, body or sequence `type` is invalid. |
| `404` | A single requested transcript was not found. |
| `500` | Required annotation or reference data is missing or inconsistent. |
