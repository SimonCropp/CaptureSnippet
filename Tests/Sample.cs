using System.IO;
using System.Linq;
using CaptureSnippets;
using Version = CaptureSnippets.Version;

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
    var readSnippets = await snippetExtractor.FromFiles(filesToParse).ConfigureAwait(false);

    // Grouping
    var snippetGroups = SnippetGrouper.Group(readSnippets)
        .ToList();

    // Merge with some markdown text
    var markdownProcessor = new MarkdownProcessor();

    //In this case the text will be extracted from a file path
    ProcessResult result;
    using (var reader = File.OpenText(@"C:\path\myInputMarkdownFile.md"))
    using (var writer = File.CreateText(@"C:\path\myOutputMarkdownFile.md"))
    {
        result = await markdownProcessor.Apply(snippetGroups, reader, writer).ConfigureAwait(false);
    }

    // List of all snippets that the markdown file expected but did not exist in the input snippets 
    var missingSnippets = result.MissingSnippets;

    // List of all snippets that the markdown file used
    var usedSnippets = result.UsedSnippets;

    }

    static Version InferVersion(string path)
    {
        var directories = path.Split(Path.DirectorySeparatorChar)
            .Reverse();
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