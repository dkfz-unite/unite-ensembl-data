# Ensembl Proteins API
Allows to find proteins by their stable ids with or without additional details.

### GET: [api/proteins/id/[id]?expand=true](http://localhost:5200/api/proteins/id/ENSP00000275493?expand=true)
Find protein by stable id.

#### Parameters
**`id`*** - Ensembl protein stable identifier.
- Type: _String_
- Example: `ENSP00000275493`

**`expand`** - Include protein features.
- Type: _Boolean_
- Example: `true`

#### Resources
- Protein data in **json** format if was found.

#### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - protein with given id was not found


### POST [api/proteins/id?expand=true](http://localhost:5200/api/proteins/id?expand=true)
Find proteins by stable ids.

#### Parameters
**`expand`** - Include protein features.
- Type: _Boolean_
- Example: `true`

#### Body
Body is an array of protein stable ids in **json** format.
```json
[
  "ENSP00000275493",
  "ENSP00000467141"
]
```

#### Resources
- Array of proteins data in **json** format if were found.

#### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `404` - proteins with given ids were not found

##
- **`*`** - Required parameter.
- `expand` - Expand functionality is different from original Ensembl API. It includes only information about protein PFAM domains. It doesn't include any other features.
