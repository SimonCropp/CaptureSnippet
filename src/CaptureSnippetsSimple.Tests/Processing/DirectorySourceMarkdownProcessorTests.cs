using System.IO;
using System.Linq;
using CaptureSnippets;
using Xunit;

public class DirectorySourceMarkdownProcessorTests : TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.Find();

        var files = Directory.EnumerateFiles(Path.Combine(root, "CaptureSnippets.Tests/Snippets"), "*.cs")
            .Concat(Directory.EnumerateFiles(Path.Combine(root, "CaptureSnippetsSimple.Tests/Snippets"), "*.cs"));
        DirectorySourceMarkdownProcessor.Run(root,files);
    }
}