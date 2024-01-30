using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

using swagger2md;

using var swaggerFileStream = Console.OpenStandardInput();
//using var swaggerFileStream = File.OpenRead("/projects/publicweb/src/VzpTheme.WebAssets/swagger.json");
var openApiDocument = new OpenApiStreamReader().Read(swaggerFileStream, out var diagnostic);

Markdown.BoldText(openApiDocument.Info.Title);
Markdown.Text(openApiDocument.Info.Description);
Markdown.Header1("Table of contents");
Markdown.TableOfContents();

var tags = new Dictionary<string, List<(string Path, string Type, OpenApiOperation Operation)>>();

// group paths by tags
foreach (var pathInfo in openApiDocument.Paths)
    foreach (var operation in pathInfo.Value.Operations)
        foreach (var tag in operation.Value.Tags)
        {
            if (tags.TryGetValue(tag.Name, out var tagsValue))
                tagsValue.Add((pathInfo.Key, operation.Key.ToString(), operation.Value));
            else
                tags.Add(tag.Name, [(pathInfo.Key, operation.Key.ToString(), operation.Value)]);
        }

Markdown.Header1("Tags");

foreach (var tagInfo in tags)
{
    Markdown.Header2(tagInfo.Key);

    foreach (var operationInfo in tagInfo.Value)
    {
        Markdown.Header3($"{operationInfo.Type} {operationInfo.Path} - {operationInfo.Operation.OperationId}");

        Markdown.Text(operationInfo.Operation.Summary);

        Markdown.BoldText("Notes");
        Markdown.Text(operationInfo.Operation.Description);

        if (operationInfo.Operation.Parameters.Count > 0)
        {
            Markdown.BoldText("Parameters");

            Console.WriteLine("| Name | Located in | Description | Required | Schema |");
            Console.WriteLine("| ---- | ---------- | ----------- | -------- | ---- |");

            foreach (var parameter in operationInfo.Operation.Parameters)
            {
                Console.WriteLine($"| {parameter.Name} |{parameter.In?.ToString().ToLower()} | {parameter.Description} | {(parameter.Required ? "Yes" : "No").ToLower()} | {parameter.Schema.GetSchemaType()} |");
            }

            Console.WriteLine();
        }

        if (operationInfo.Operation.Responses.Count > 0)
        {
            Markdown.BoldText("Responses");

            Console.WriteLine("| Code | Description | Format | Schema |");
            Console.WriteLine("| ---- | ----------- | ------ | ------ |");

            foreach (var response in operationInfo.Operation.Responses)
            {
                foreach (var contentInfo in response.Value.Content)
                {
                    var schema = contentInfo.Value.Schema.GetSchemaType();

                    Console.WriteLine($"| {response.Key} | {response.Value.Description} | {string.Join(",", contentInfo.Key)} | {schema} |");
                }
            }
        }

        Console.WriteLine();
    }
}

Markdown.Header1("Models");

foreach (var schemeInfo in openApiDocument.Components.Schemas)
{
    Markdown.Header2(schemeInfo.Key);

    if (schemeInfo.Value.Description is not null)
    {
        Markdown.Text(schemeInfo.Value.Description);
    }

    Console.WriteLine("| Name | Type | Description | Required |");
    Console.WriteLine("| ---- | ---- | ----------- | -------- |");

    foreach (var parameterInfo in schemeInfo.Value.Properties)
    {
        var isParameterRequired = schemeInfo.Value.Required.Contains(parameterInfo.Key) ? "Yes" : "No";
        var description = parameterInfo.Value?.Enum?.Count > 0 // it is enum type
            ? $"_Enum_: {string.Join(",", parameterInfo.Value.Enum.Select(enumValue =>
            {
                if (enumValue is OpenApiString openApiString)
                    return $"\"{openApiString.Value}\"";

                return "No string values, improve api result!";
            }))}"
            : parameterInfo.Value?.Description;

        Console.WriteLine($"| {parameterInfo.Key} | {parameterInfo.Value?.GetSchemaType()} | {description} | {isParameterRequired} |");
    }

    Console.WriteLine();
}