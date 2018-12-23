using System.IO;
using CaptureSnippets;
using Xunit;

public class GirRepoDirectoryFinderTests
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        Assert.True(GirRepoDirectoryFinder.TryFind(out var path));
        Assert.True(Directory.Exists(path));
    }
}