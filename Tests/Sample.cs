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

        // setup version convention and extract snippets from files

        var snippetExtractor = new DirectorySnippetExtractor(
            extractMetaData: InferMetaData,
            includeDirectory: IncludeDirectory,
            includeFile: IncludeFile);
        var readSnippets = await snippetExtractor.FromDirectory(@"C:\path");

        // Grouping
        var snippetGroups = SnippetGrouper.Group(readSnippets)
            .ToList();

        //In this case the text will be extracted from a file path
        ProcessResult result;
        using (var reader = File.OpenText(@"C:\path\myInputMarkdownFile.md"))
        using (var writer = File.CreateText(@"C:\path\myOutputMarkdownFile.md"))
        {
            result = await MarkdownProcessor.Apply(snippetGroups, reader, writer, SimpleMarkdownHandling.AppendGroup);
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

    Result<string> InferPackage(string path, string parent)
    {
        throw new System.NotImplementedException();
    }

    bool IncludeFile(string filepath)
    {
        return filepath.EndsWith(".vm") || filepath.EndsWith(".cs");
    }

    static Result<SnippetMetaData> InferMetaData(string rootPath, string path, SnippetMetaData parent)
    {
        VersionRange version;
        var split = path.Split('_');
        if (VersionRange.TryParse(split[1], out version))
        {
            return new SnippetMetaData(version, split[0]);
        }
        return parent;
    }
}