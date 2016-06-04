using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaptureSnippets;
using NuGet.Versioning;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

class Sample
{

    async Task UseSnippetExtractor()
    {

        // setup version convention and extract snippets from files

        var snippetExtractor = new DirectorySnippetExtractor(
            extractPathData: InferVersionAndPath,
            includeDirectory: IncludeDirectory,
            includeFile: IncludeFile);
        var readSnippets = await snippetExtractor.FromDirectory(@"C:\path");

        // Grouping
        var snippetGroups = SnippetGrouper.Group(readSnippets)
            .ToList();

        //In this case the text will be extracted from a file path
        ProcessResult result;
        var markdownProcessor = new MarkdownProcessor(
            snippets: snippetGroups,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            includes: new List<IncludeGroup>(),
            appendIncludeGroup: (group, writer) => { throw new Exception(); });
        using (var reader = File.OpenText(@"C:\path\myInputMarkdownFile.md"))
        using (var writer = File.CreateText(@"C:\path\myOutputMarkdownFile.md"))
        {
            result = await markdownProcessor.Apply(reader, writer);
        }

        // List of all snippets that the markdown file expected but did not exist in the input snippets
        var missingSnippets = result.Missing;

        // List of all snippets that the markdown file used
        var usedSnippets = result.UsedSnippets;

    }

    bool IncludeDirectory(string directorypath)
    {
        throw new System.NotImplementedException();
    }

    Package InferPackage(string path, string parent)
    {
        throw new System.NotImplementedException();
    }

    bool IncludeFile(string filepath)
    {
        return filepath.EndsWith(".vm") || filepath.EndsWith(".cs");
    }

    static PathData InferVersionAndPath(string path)
    {
        VersionRange version;
        var split = path.Split('_');
        if (VersionRange.TryParse(split[1], out version))
        {
            return PathData.With(version, split[0]);
        }
        return PathData.WithParent();
    }
}