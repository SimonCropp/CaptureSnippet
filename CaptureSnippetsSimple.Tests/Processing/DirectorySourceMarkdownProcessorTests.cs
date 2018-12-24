using CaptureSnippets;
using Xunit;


public class DirectorySourceMarkdownProcessorTests : TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.Find();
        //DirectorySourceMarkdownProcessor.Run(root,x=>);
    }
}