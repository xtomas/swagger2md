# swagger2md

Swagger to azure wiki markdown conversion utility based on OpenApiDocument definition.

### Features

* Optional add OpenApiDocument Title
* Add OpenApiDocument Description
* group all OpenApiDocument paths by tags
* list of all tags, paths, operations, operation parameters, operation responses
* list of all schemes
* links between parameter schemas and schemes list
* optional subpages generation

### Build project

```
dotnet build
```

### Usage

PowerShell

```PowerShell
./swagger2md -i /projects/webapp/swagger.json -o api.md -sp true -st false
```

CMD
```Batchfile
./swagger2md -i /projects/webapp/swagger.json -o api.md -sp true -st false
```

### Parameters

| Parameter short | Parameter full | Description |
| --  | -- | -- |
| -i  | --inputFile | required full or relative path of the swagger.json file |
| -st | --skipTitle| if true, open api document title will be skipped |
| -o  | --outputFile | optional input's file full or relative path (if empty output is standard output) |
| -sp | --subPages| optional genration tags into subpages (files in subfolder), outputFile is required |

