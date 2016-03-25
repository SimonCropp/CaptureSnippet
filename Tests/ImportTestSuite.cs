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
    public async void RunScenarios()
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
        var extractor = new FileSnippetExtractor((x, y) => VersionRange.All, (x, y) => null);
        var path = Path.Combine(folder, "code.cs");
        using (var textReader = File.OpenText(path))
        {
            await extractor.AppendFromReader(textReader, path, VersionRange.All, null, snippets.Add);
        }

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        using (var reader = File.OpenText(input))
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                await MarkdownProcessor.Apply(snippetGroups, reader, writer, SimpleMarkdownHandling.AppendGroup);
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
