using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class ImportTestSuite
{
    [Test]
    public void RunScenarios()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        var folders = Directory.GetDirectories(directory);

        foreach (var folder in folders)
        {
            var input = Path.Combine(folder, "input.md");
            var output = Path.Combine(folder, "output.md");
            Run(folder, input, output);
        }
    }

    void Run(string folder, string input, string expectedOutput)
    {
        var snippets = new List<ReadSnippet>();
        var data = PathData.With(VersionRange.All, Package.Undefined, Component.Undefined);
        var extractor = new FileSnippetExtractor(y => data);
        var path = Path.Combine(folder, "code.cs");
        using (var textReader = File.OpenText(path))
        {
            extractor.AppendFromReader(textReader, path, VersionRange.All, Package.Undefined, Component.Undefined, snippets.Add);
        }

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        var markdownProcessor = new MarkdownProcessor(
            snippets: snippetGroups,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup);
        using (var reader = File.OpenText(input))
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                markdownProcessor.Apply(reader, writer);
            }

            var expected = File.ReadAllText(expectedOutput).FixNewLines();
            var fixNewLines = stringBuilder
                .ToString()
                .FixNewLines()
                .TrimTrailingNewLine();
            Assert.AreEqual(expected, fixNewLines, folder);
        }
    }

}
