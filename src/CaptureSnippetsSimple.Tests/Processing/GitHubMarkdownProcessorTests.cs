using System.IO;
using System.Linq;
using CaptureSnippets;
using Xunit;

public class GitHubMarkdownProcessorTests : TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var files = Directory.EnumerateFiles(Path.Combine(root, "src/CaptureSnippets.Tests/Snippets"), "*.cs")
            .Concat(Directory.EnumerateFiles(Path.Combine(root, "src/CaptureSnippetsSimple.Tests/Snippets"), "*.cs"));
        GitHubMarkdownProcessor.Run(root, files.ToList());
    }
}