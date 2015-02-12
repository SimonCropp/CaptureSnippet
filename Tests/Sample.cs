using System.IO;
using System.Linq;
using System.Text;
using CaptureSnippets;

class Sample
{
    void ConstructSnippetExtractorWIthCustomVersionInferer()
    {
        var snippetExtractor = new SnippetExtractor(InferVersion);
    }


    async void UseSnippetExtractor()
    {
        // get files containing snippets
        var filesToParse = Directory.EnumerateFiles(@"C:\path", "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".vm") || s.EndsWith(".cs"));

        // setup version convention and extract snippets from files
        var snippetExtractor = new SnippetExtractor(InferVersion);
        var readSnippets = snippetExtractor.FromFiles(filesToParse);

        // Grouping
        var snippetGroups = SnippetGrouper.Group(readSnippets)
            .ToList();

        // Merge with some markdown text
        var markdownProcessor = new MarkdownProcessor();

        //In this case the text will be extracted from a file path
        ProcessResult result;
        var stringBuilder = new StringBuilder();
        using (var reader = File.OpenText(@"C:\path\mymarkdownfile.md"))
        using (var writer = new StringWriter(stringBuilder))
        {
            result = await markdownProcessor.Apply(snippetGroups, reader, writer);
        }

        // List of all snippets that the markdown file expected but did not exist in the input snippets 
        var missingSnippets = result.MissingSnippet;

        // List of all snippets that the markdown file used
        var usedSnippets = result.UsedSnippets;

        // The resultant markdown of merging the snippets with the markdown file
        var text = stringBuilder.ToString();
    }

    static Version InferVersion(string path)
    {
        var directories = path.Split(Path.DirectorySeparatorChar, Path.DirectorySeparatorChar).Reverse();
        foreach (var directory in directories)
        {
            Version version;
            if (VersionParser.TryParseVersion(directory.Split('_').Last(), out version))
            {
                return version;
            }
        }

        return null;
    }
}