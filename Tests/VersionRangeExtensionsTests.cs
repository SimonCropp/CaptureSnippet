using CaptureSnippets;
using NuGet.Versioning;
using Xunit;

public class VersionRangeExtensionsTests
{
    [Fact]
    public void ToFriendlyString()
    {
        Assert.Equal("> 1.0.0 && < 2.0.0", VersionRange.Parse("(1.0,2.0)").ToFriendlyString());
        Assert.Equal("all", VersionRange.All.ToFriendlyString());
        Assert.Equal("none", VersionRange.None.ToFriendlyString());
    }

    [Fact]
    public void PrettyPrintVersion()
    {
        Assert.Equal("4-pre", NuGetVersion.Parse("4.0-pre").SimplePrint());
    }

    [Fact]
    public void PrettyPrintVersionRange()
    {
        Assert.Equal("1.1", VersionRange.Parse("[1.1,2.0)").SimplePrint());
        Assert.Equal("0.2", VersionRange.Parse("[0.2.0, 1.0.0)").SimplePrint());
        Assert.Equal("1-pre", VersionRange.Parse("[1.0.0-pre, 2.0.0)").SimplePrint());

        Assert.Equal("0.2", VersionRange.Parse("[0.2,0.3)").SimplePrint());
        Assert.Equal("All", VersionRange.All.SimplePrint());
        Assert.Equal("None", VersionRange.None.SimplePrint());
        Assert.Equal("1.1 - 3.0", VersionRange.Parse("(1.0,3.1)").SimplePrint());
        Assert.Equal("1.x - 3.0", VersionRange.Parse("[1.0,3.1)").SimplePrint());
        Assert.Equal("1.x - 3.1", VersionRange.Parse("[1.0,3.1]").SimplePrint());
        Assert.Equal("1.1 - 3.1", VersionRange.Parse("(1.0,3.1]").SimplePrint());

        Assert.Equal("1.1 - 2.x", VersionRange.Parse("(1.0,3.0)").SimplePrint());
        Assert.Equal("1.x - 2.x", VersionRange.Parse("[1.0,3.0)").SimplePrint());
        Assert.Equal("1.x - 3.x", VersionRange.Parse("[1.0,3.0]").SimplePrint());
        Assert.Equal("1.1 - 3.x", VersionRange.Parse("(1.0,3.0]").SimplePrint());

        Assert.Equal("1.1", VersionRange.Parse("(1.0,2.0)").SimplePrint());
        Assert.Equal("1.x", VersionRange.Parse("[1.0,2.0)").SimplePrint());
        Assert.Equal("1.x - 2.x", VersionRange.Parse("[1.0,2.0]").SimplePrint());
        Assert.Equal("1.1 - 2.x", VersionRange.Parse("(1.0,2.0]").SimplePrint());

        Assert.Equal("1-pre - N", VersionRange.Parse("[1.0-pre,)").SimplePrint());
        Assert.Equal("1-alpha - N", VersionRange.Parse("[1.0-alpha,)").SimplePrint());

        Assert.Equal("1.1 - 2.0", VersionRange.Parse("(1.0,2.1)").SimplePrint());
        Assert.Equal("1.x - 2.0", VersionRange.Parse("[1.0,2.1)").SimplePrint());
        Assert.Equal("1.x - 2.1", VersionRange.Parse("[1.0,2.1]").SimplePrint());
        Assert.Equal("1.1 - 2.1", VersionRange.Parse("(1.0,2.1]").SimplePrint());

        Assert.Equal("1.1 - N", VersionRange.Parse("(1.0,]").SimplePrint());
        Assert.Equal("1.x - N", VersionRange.Parse("[1.0,]").SimplePrint());
        Assert.Equal("N - 2.0", VersionRange.Parse("[,2.1)").SimplePrint());
        Assert.Equal("N - 2.1", VersionRange.Parse("[,2.1]").SimplePrint());
    }
}