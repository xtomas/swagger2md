using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using swagger2md.Helpers;
using System.Diagnostics.CodeAnalysis;

public class DocumentGenerator
{
    private readonly IConfiguration configuration;

    public DocumentGenerator(IConfiguration configuration) => this.configuration = configuration;

    public void GenerateDocument()
    {
        var inputFile = configuration["input"];

        if (!TryGetFileStream(out var swaggerFileStream))
        {
            Console.WriteLine($"Input file not found. Use -i or --input argument.");
            return;
        }

        var outputFile = configuration["outputFile"];
        var openApiDocument = new OpenApiStreamReader().Read(swaggerFileStream, out var diagnostic);
        var textWriter = GetTargetFileTextWriter(outputFile);

        if (!string.Equals(configuration["skipTitle"], bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
            MarkdownHelper.BoldText(textWriter, openApiDocument.Info.Title);
        }

        MarkdownHelper.Text(textWriter, openApiDocument.Info.Description);

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

        var generateSubFiles = string.Equals(configuration["subPages"], bool.TrueString, StringComparison.OrdinalIgnoreCase);
        var outputDir = Path.GetDirectoryName(outputFile) ?? ".";

         if (generateSubFiles)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine("Can not generate subfiles without outputFile property.");
                return;
            }
            else
            {
                MarkdownHelper.TableOfSubpages(textWriter);
                outputDir = Directory.CreateDirectory(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(outputFile).Replace(' ', '-'))).FullName;
            }
        }
        else
        {
            MarkdownHelper.TableOfContents(textWriter);
        }

        foreach (var tagInfo in tags)
        {
            using var tagTextWriter = generateSubFiles && outputDir is not null
                ? File.CreateText(Path.Combine(outputDir, $"{tagInfo.Key.Replace(' ', '-')}.md"))
                : textWriter;

            MarkdownHelper.Header1(tagTextWriter, tagInfo.Key);

            if (textWriter != tagTextWriter)
            {
                MarkdownHelper.TableOfContents(tagTextWriter);
            }

            foreach (var operationInfo in tagInfo.Value)
            {
                OpenApiHelpers.WriteOperationInfo(tagTextWriter, operationInfo.Path, operationInfo.Type, operationInfo.Operation, openApiDocument.Components.Schemas);

                tagTextWriter.WriteLine();
            }

            if (textWriter != tagTextWriter)
            {
                tagTextWriter.Close();
            }
        }

        textWriter.Close();
    }

    private bool TryGetFileStream([NotNullWhen(true)] out Stream? fileStream)
    {
        var inputFile = configuration["input"];

        if (File.Exists(inputFile))
        {
            fileStream = File.OpenRead(inputFile);
            return true;
        }

        fileStream = default;
        return false;
    }

    private static TextWriter GetTargetFileTextWriter(string? outputFile)
    {
        if (!string.IsNullOrEmpty(outputFile))
        {
            return File.CreateText(outputFile);
        }

        return Console.Out;
    }
}

