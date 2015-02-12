using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class ImportTestSuite
{
    [Test]
    public async void RunScenarios()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        var folders = Directory.GetDirectories(directory);

        foreach (var folder in folders)
        {
            await Run(folder, Path.Combine(folder, "input.md"), Path.Combine(folder, "output.md"));
        }
    }

    async Task Run(string folder, string input, string expectedOutput)
    {
        var snippets = new SnippetExtractor().FromFiles(Directory.EnumerateFiles(folder, "code.cs"));

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        using (var reader = File.OpenText(input))
        {
            var markdownProcessor = new MarkdownProcessor();
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                await markdownProcessor.Apply(snippetGroups, reader, writer);
            }

            var expected = File.ReadAllText(expectedOutput).FixNewLines();
            var fixNewLines = stringBuilder.ToString().FixNewLines().TrimTrailingNewLine();
            Assert.AreEqual(expected, fixNewLines, folder);
        }
    }

}
