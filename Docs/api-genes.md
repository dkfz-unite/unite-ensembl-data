# Ensembl Genes API
Allows to find genes by their stable ids or well known names with or without additional details.

## GET: [api/genes/id/[id]?length=false&expand=true](http://localhost:5200/api/genes/id/ENSG00000146648?length=false&expand=true)
Find gene by stable id.

### Parameters
**`id`*** - Ensembl gene stable identifier.
- Type: _String_
- Example: `ENSG00000146648`

**`length`** - Include gene exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include gene details such as canonical transcript it's protein and features.
- Type: _Boolean_
- Example: `true`

### Resources
- Gene data in **json** format if was found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - gene with given id was not found


## POST [api/genes/id?length=false&expand=true](http://localhost:5200/api/genes/id?length=false&expand=true)
Find genes by stable ids.

### Parameters
**`length`** - Include gene exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include gene details such as canonical transcript it's protein and features.
- Type: _Boolean_
- Example: `true`

### Body
Body is an array of gene stable ids in **json** format.
```json
[
  "ENSG00000146648",
  "ENSG00000155657"
]
```

### Resources
- Array of genes data in **json** format if were found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - genes with given ids were not found


## GET: [api/genes/symbol/[symbol]?length=false&expand=true](http://localhost:5200/api/genes/symbol/EGFR?length=false&expand=true)
Find gene by it's well known name.

### Parameters
**`symbol`*** - Gene symbol.
- Type: _String_
- Example: `EGFR`

**`length`** - Include gene exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include gene details such as canonical transcript it's protein and features.
- Type: _Boolean_
- Example: `true`

### Resources
- Gene data in **json** format if was found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - gene with given symbol was not found


## POST [api/genes/symbol?length=false&expand=true](http://localhost:5200/api/genes/symbol?length=false&expand=true)
Find genes by their well known names.

### Parameters
**`length`** - Include gene exonic length.
- Type: _Boolean_
- Example: `false`

**`expand`** - Include gene details such as canonical transcript it's protein and features.
- Type: _Boolean_
- Example: `true`

### Body
Body is an array of gene symbols in **json** format.
```json
[
  "EGFR",
  "TTN"
]
```

### Resources
- Array of genes data in **json** format if were found.

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - genes with given symbols were not found


##
- **`*`** - Required parameter.
- `length` - Gene exonic length is an exonic length of it's canonical transcript. The value has to be calculated so it may slow down the response for a big amount of data or large genes.
- `expand` - Expand functionality is different from original Ensembl API. It includes only information about gene canonical transcript, it's protein and features. It doesn't include information about other transcripts, exons, introns, etc.
