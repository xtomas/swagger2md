using Microsoft.OpenApi.Models;

namespace swagger2md;

public static class OpenApiHelpers
{
    /// <summary>
    /// Get schema text or link to model
    /// </summary>
    public static string GetSchemaType(this OpenApiSchema? schema)
    {
        if (schema is null)
            return string.Empty;

        return schema.Type switch
        {
            "array" => schema.Items?.Type == "object"
                ? $"[ [{schema.Items?.Reference?.Id}](#{schema.Items?.Reference?.Id}) ]"
                : $"[ {schema.Items?.Type} ]",
            "object" => $"[{schema.Reference?.Id}](#{schema.Reference?.Id})",
            _ => schema.Type
        };
    }
}