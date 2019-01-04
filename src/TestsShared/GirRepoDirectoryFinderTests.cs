using System.IO;
using CaptureSnippets;
using Xunit;

public class GirRepoDirectoryFinderTests
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        Assert.True(GitRepoDirectoryFinder.TryFind(out var path));
        Assert.True(Directory.Exists(path));
    }
}