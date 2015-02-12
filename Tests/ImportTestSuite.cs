using System.IO;
using System.Linq;
using CaptureSnippets;
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
            Run(folder, Path.Combine(folder, "input.md"), Path.Combine(folder, "output.md"));
        }
    }

    void Run(string folder, string input, string expectedOutput)
    {

        var snippets = new SnippetExtractor().FromFiles(Directory.EnumerateFiles(folder, "code.cs"));

        var snippetGroups = SnippetGrouper.Group(snippets)
            .ToList();

        using (var reader = File.OpenText(input))
        {
            var result = new MarkdownProcessor().Apply(snippetGroups, reader);

            var expected = File.ReadAllText(expectedOutput).FixNewLines();
            var fixNewLines = result.Text.FixNewLines();
            Assert.AreEqual(expected, fixNewLines, folder);
        }
    }

}
