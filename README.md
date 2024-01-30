# swagger2md

Swagger to azure wiki markdown conversion utility based on OpenApiDocument definition.

### Features

* Add OpenApiDocument Title
* Add OpenApiDocument Description
* group all OpenApiDocument paths by tags
* list of all tags, paths, operations, operation parameters, operation responses
* list of all schemes
* links between parameter schemas and schemes list

### Build project

```
dotnet build
```

### Usage

PowerShell

```PowerShell
Get-Content "path-to-swagger/swagger.json" | swagger2md > api.md
```

CMD
```Batchfile
swagger2md < "path-to-swagger/swagger.json" > api.md
```
