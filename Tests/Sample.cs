using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaptureSnippets;
using NuGet.Versioning;
// ReSharper disable UnusedMember.Local

class Sample
{

    async Task UseSnippetExtractor()
    {

        // setup version convention and extract snippets from fi`les
        var snippetExtractor = new DirectorySnippetExtractor(
            extractVersion: InferVersion,
            extractPackage: InferPackage,
            includeDirectory: IncludeDirectory,
            includeFile: IncludeFile);
        var readSnippets = await snippetExtractor.FromDirectory(@"C:\path");

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
            result = await markdownProcessor.Apply(snippetGroups, reader, writer);
        }

        // List of all snippets that the markdown file expected but did not exist in the input snippets 
        var missingSnippets = result.MissingSnippets;

        // List of all snippets that the markdown file used
        var usedSnippets = result.UsedSnippets;

    }

    bool IncludeDirectory(string directorypath)
    {
        throw new System.NotImplementedException();
    }

    string InferPackage(string path, string parent)
    {
        throw new System.NotImplementedException();
    }

    bool IncludeFile(string filepath)
    {
        return filepath.EndsWith(".vm") || filepath.EndsWith(".cs");
    }

    static VersionRange InferVersion(string path, VersionRange parent)
    {
        VersionRange version;
        if (VersionRange.TryParse(path.Split('_').Last(), out version))
        {
            return version;
        }

        return parent;
    }
}