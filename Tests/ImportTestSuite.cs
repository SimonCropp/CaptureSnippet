using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class ImportTestSuite
{
    [Test]
    public async Task RunScenarios()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        var folders = Directory.GetDirectories(directory);

        foreach (var folder in folders)
        {
            var input = Path.Combine(folder, "input.md");
            var output = Path.Combine(folder, "output.md");
            await Run(folder, input, output);
        }
    }

    async Task Run(string folder, string input, string expectedOutput)
    {
        var snippets = new List<ReadSnippet>();
        var data = PathData.With(VersionRange.All, Package.Undefined);
        var extractor = new FileSnippetExtractor(y => data);
        var path = Path.Combine(folder, "code.cs");
        using (var textReader = File.OpenText(path))
        {
            await extractor.AppendFromReader(textReader, path, VersionRange.All, Package.Undefined, snippets.Add);
        }

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        var markdownProcessor = new MarkdownProcessor(
            snippets: snippetGroups,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            includes: new List<IncludeGroup>(),
            appendIncludeGroup: (group, writer) => {throw new Exception();} );
        using (var reader = File.OpenText(input))
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                await markdownProcessor.Apply(reader, writer);
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
