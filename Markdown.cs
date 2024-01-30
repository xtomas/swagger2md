namespace swagger2md;

public static class Markdown
{
    public static void Header1(string header)
    {
        Console.WriteLine($"# {header}");
        Console.WriteLine("---");
        Console.WriteLine();
    }

    public static void Header2(string header)
    {
        Console.WriteLine($"## {header}");
        Console.WriteLine();
    }

    public static void Header3(string header)
    {
        Console.WriteLine($"### {header}");
        Console.WriteLine();
    }

    public static void TableOfContents()
    {
        Console.WriteLine("[[_TOC_]]"); // add table of contents (azure wiki specific)
        Console.WriteLine();
    }

    public static void BoldText(string text)
    {
        Console.WriteLine($"**{text}**");
        Console.WriteLine();
    }

    public static void Text(string text)
    {
        Console.WriteLine(text);
        Console.WriteLine();
    }
}
