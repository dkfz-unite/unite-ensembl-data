# Ensembl Transcripts API
Allows to find transcripts by their stable ids or well known names with or without additional details.

## GET: [api/transcripts/id/[id]?length=false&expand=true](http://localhost:5200/api/transcripts/id/ENST00000275493?length=false&expand=true)
Find transcript by stable id.

### Parameters
**`id`*** - Ensembl transcript stable identifier.
- Type: _String_
- Example: `ENST00000275493`

**`length`** - Include transcript exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include transcript details such as protein and it's features.
- Type: _Boolean_
- Example: `true`

### Resources
- Transcript data in **json** format if was found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - transcript with given id was not found


## POST [api/transcripts/id?length=false&expand=true](http://localhost:5200/api/transcripts/id?length=false&expand=true)
Find transcripts by stable ids.

### Parameters
**`length`** - Include transcript exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include transcripts details such protein and it's features.
- Type: _Boolean_
- Example: `true`

### Body
Body is an array of transcript stable ids in **json** format.
```json
[
  "ENST00000275493",
  "ENST00000589042"
]
```

### Resources
- Array of transcripts data in **json** format if were found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - transcripts with given ids were not found


## GET: [api/transcripts/symbol/[symbol]?length=false&expand=true](http://localhost:5200/api/transcripts/symbol/EGFR-201?length=false&expand=true)
Find transcript by well known name.

### Parameters
**`symbol`*** - Transcript well known name.
- Type: _String_
- Example: `EGFR`

**`length`** - Include transcript exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include transcripts details such protein and it's features.
- Type: _Boolean_
- Example: `true`

### Resources
- Transcript data in **json** format if was found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - transcript with given symbol was not found


## POST: [api/transcripts/symbol?length=false&expand=true](http://localhost:5200/api/transcripts/symbol?length=false&expand=true)
Find transcripts by well known names.

### Parameters
**`length`** - Include transcript exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include transcripts details such protein and it's features.
- Type: _Boolean_
- Example: `true`

### Body
Body is an array of transcript well known names in **json** format.
```json
[
  "EGFR-201",
  "TTN-214"
]
```

### Resources
- Array of transcripts data in **json** format if were found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - transcripts with given symbols were not found


##
- **`*`** - Required parameter.
- `length` - The value has to be calculated so it may slow down the response for a big amount of data or large transcripts.
- `expand` - Expand functionality is different from original Ensembl API. It includes only information protein and it's features. It doesn't include information about exons, introns, etc.
