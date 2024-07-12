namespace swagger2md.Helpers;

public static class MarkdownHelper
{
    public static void Header1(TextWriter textWriter, string header)
    {
        textWriter.WriteLine($"# {header}");
        textWriter.WriteLine("---");
        textWriter.WriteLine();
    }

    public static void Header2(TextWriter textWriter, string header)
    {
        textWriter.WriteLine($"## {header}");
        textWriter.WriteLine();
    }

    public static void Header3(TextWriter textWriter, string header)
    {
        textWriter.WriteLine($"### {header}");
        textWriter.WriteLine();
    }

    public static void Header4(TextWriter textWriter, string header)
    {
        textWriter.WriteLine($"#### {header}");
        textWriter.WriteLine();
    }

    public static void TableOfContents(TextWriter textWriter)
    {
        textWriter.WriteLine("[[_TOC_]]"); // add table of contents (azure wiki specific)
        textWriter.WriteLine();
    }

    public static void TableOfSubpages(TextWriter textWriter)
    {
        textWriter.WriteLine("[[_TOSP_]]"); // add table of subpages (azure wiki specific)
        textWriter.WriteLine();
    }

    public static void BoldText(TextWriter textWriter, string text)
    {
        textWriter.WriteLine($"**{text}**");
        textWriter.WriteLine();
    }

    public static void Text(TextWriter textWriter, string text)
    {
        textWriter.WriteLine(text);
        textWriter.WriteLine();
    }
}
