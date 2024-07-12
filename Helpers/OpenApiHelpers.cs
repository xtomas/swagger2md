using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace swagger2md.Helpers;

public static class OpenApiHelpers
{
    /// <summary>
    /// Get schema text or link to model
    /// </summary>
    public static (string? Key, string Title) GetSchemaType(this OpenApiSchema? schema)
    {
        if (schema is null)
            return (null, string.Empty);

        return schema.Type switch
        {
            "array" => schema.Items?.Type == "object"
                ? (schema.Items?.Reference?.Id, $"[ [{schema.Items?.Reference?.Id}](#{schema.Items?.Reference?.Id}) ]")
                : (null, $"[ {schema.Items?.Type} ]"),
            "object" => (schema.Reference?.Id, $"[{schema.Reference?.Id}](#{schema.Reference?.Id})"),
            _ => (null, schema.Type)
        };
    }

    /// <summary>
    /// Write open api operation informations into given text writer
    /// </summary>
    public static void WriteOperationInfo(this TextWriter textWriter, string path, string type, OpenApiOperation operation, IDictionary<string, OpenApiSchema> schemas)
    {
        MarkdownHelper.Header2(textWriter, $"{type} {path} - {operation.OperationId}");

        MarkdownHelper.Text(textWriter, operation.Summary);

        MarkdownHelper.Header3(textWriter, "Notes");
        MarkdownHelper.Text(textWriter, operation.Description);

        var operationModels = new List<string>();

        if (operation.Parameters.Count > 0)
        {
            MarkdownHelper.BoldText(textWriter, "Parameters");

            textWriter.WriteLine("| Name | Located in | Description | Required | Schema |");
            textWriter.WriteLine("| ---- | ---------- | ----------- | -------- | ------ |");

            foreach (var parameter in operation.Parameters)
            {
                var schema = parameter.Schema.GetSchemaType();

                if (schema.Key is not null)
                {
                    operationModels.Add(schema.Key);
                }

                textWriter.WriteLine($"| {parameter.Name} |{parameter.In?.ToString().ToLower()} | {parameter.Description} | {(parameter.Required ? "Yes" : "No").ToLower()} | {schema.Title} |");
            }

            textWriter.WriteLine();
        }

        if (operation.Responses.Count > 0)
        {
            MarkdownHelper.Header3(textWriter, "Responses");

            textWriter.WriteLine("| Code | Description | Format | Schema |");
            textWriter.WriteLine("| ---- | ----------- | ------ | ------ |");

            foreach (var response in operation.Responses)
            {
                foreach (var contentInfo in response.Value.Content)
                {
                    var schema = contentInfo.Value.Schema.GetSchemaType();
                    var format = string.IsNullOrEmpty(schema.Title) ? null : string.Join(",", contentInfo.Key);

                    if (!string.IsNullOrEmpty(schema.Key))
                    {
                        operationModels.Add(schema.Key);
                    }

                    textWriter.WriteLine($"| {response.Key} | {response.Value.Description} | {format} | {schema.Title} |");
                }
            }
        }

        if (operationModels.Any())
        {
            textWriter.WriteLine();

            MarkdownHelper.Header3(textWriter, "Models");

            foreach (var schemaKey in operationModels.Distinct())
            {
                if (schemas.TryGetValue(schemaKey, out var schema))
                {
                    WriteModelInfo(textWriter, schemaKey, schema, schemas, level: 4);
                }
            }
        }
    }

    /// <summary>
    /// Write open api schema informations into given text writer
    /// </summary>
    public static void WriteModelInfo(this TextWriter textWriter, string modelName, OpenApiSchema schema, IDictionary<string, OpenApiSchema> schemas, int level = 3)
    {
        switch (level)
        {
            case 1:
                MarkdownHelper.Header1(textWriter, modelName);
                break;
            case 2:
                MarkdownHelper.Header2(textWriter, modelName);
                break;
            case 3:
                MarkdownHelper.Header3(textWriter, modelName);
                break;
            default:
                MarkdownHelper.Header4(textWriter, modelName);
                break;
        };

        if (schema.Description is not null)
        {
            MarkdownHelper.Text(textWriter, schema.Description);
        }

        textWriter.WriteLine("| Name | Type | Description | Required |");
        textWriter.WriteLine("| ---- | ---- | ----------- | -------- |");

        var parameterSchemas = new List<string>();

        foreach (var parameterInfo in schema.Properties)
        {
            var isParameterRequired = schema.Required.Contains(parameterInfo.Key) ? "Yes" : "No";
            var description = parameterInfo.Value?.Enum?.Count > 0 // it is enum type
                ? $"_Enum_: {string.Join(",", parameterInfo.Value.Enum.Select(enumValue =>
                {
                    if (enumValue is OpenApiString openApiString)
                        return $"\"{openApiString.Value}\"";

                    return "No string values, improve api result!";
                }))}"
                : parameterInfo.Value?.Description;
            var parameterSchema = parameterInfo.Value?.GetSchemaType();

            if (parameterSchema != null
                && parameterSchema.Value.Key != null
                && !string.Equals(parameterSchema.Value.Key, modelName, StringComparison.OrdinalIgnoreCase)) // prevent infinite loop
            {
                parameterSchemas.Add(parameterSchema.Value.Key);
            }

            textWriter.WriteLine($"| {parameterInfo.Key} | {parameterSchema?.Title} | {description} | {isParameterRequired} |");
        }

        foreach (var parameterSchema in parameterSchemas)
        {
            WriteModelInfo(textWriter, parameterSchema, schemas[parameterSchema], schemas, level: 4);

            textWriter.WriteLine();
        }
    }
}