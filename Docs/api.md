# Ensembl Data API

The service provides gene, transcript and protein metadata and sequences from
the installed Ensembl GRCh37 and GRCh38 core databases.

## Genome routes

The reference genome is selected by the API route prefix:

- `/api/grch37/...` uses the GRCh37 database.
- `/api/grch38/...` uses the GRCh38 database.
- `/api/...` without a genome prefix is a direct alias for GRCh37.

## Health check

`GET /api`

Returns the current server date and time as a JSON value when the service is
running.

Example response:

```json
"2026-11-09T14:25:30.123456+01:00"
```

## Feature APIs

- [Genes](./api-genes.md)
- [Transcripts](./api-transcripts.md)
- [Proteins](./api-proteins.md)
