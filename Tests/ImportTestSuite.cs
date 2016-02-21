using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
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
            Run(folder, input, output).GetAwaiter().GetResult();
        }
    }

    async Task Run(string folder, string input, string expectedOutput)
    {
        var extractor = new SnippetExtractor(s => VersionRange.All);
        var snippets = await extractor.FromFiles(Directory.EnumerateFiles(folder, "code.cs"));

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        using (var reader = File.OpenText(input))
        {
            var markdownProcessor = new MarkdownProcessor(alsoParseImportSyntax:true);
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                await markdownProcessor.Apply(snippetGroups, reader, writer);
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
