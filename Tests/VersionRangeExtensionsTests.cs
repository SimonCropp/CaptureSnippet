using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class VersionRangeExtensionsTests
{


    [Test]
    public void ToFriendlyString()
    {
        Assert.AreEqual("> 1.0.0 && < 2.0.0", VersionRange.Parse("(1.0,2.0)").ToFriendlyString());
        Assert.AreEqual("all", VersionRange.All.ToFriendlyString());
        Assert.AreEqual("none", VersionRange.None.ToFriendlyString());
    }


    [Test]
    public void PrettyPrintVersion()
    {
        Assert.AreEqual("4-pre", NuGetVersion.Parse("4.0-pre").SimplePrint());
    }

    [Test]
    public void PrettyPrintVersionRange()
    {
        Assert.AreEqual("0.2.x", VersionRange.Parse("[0.2.0, 1.0.0)").SimplePrint());
        Assert.AreEqual("1-pre", VersionRange.Parse("[1.0.0-pre, 2.0.0)").SimplePrint());

        //TODO:
        // Assert.AreEqual("0.2.x", VersionRange.Parse("[0.2,0.3)").SimplePrint());
        Assert.AreEqual("All", VersionRange.All.SimplePrint());
        Assert.AreEqual("None", VersionRange.None.SimplePrint());
        Assert.AreEqual("1.1.x - 3.0.x", VersionRange.Parse("(1.0,3.1)").SimplePrint());
        Assert.AreEqual("1.x - 3.0.x", VersionRange.Parse("[1.0,3.1)").SimplePrint());
        Assert.AreEqual("1.x - 3.1.x", VersionRange.Parse("[1.0,3.1]").SimplePrint());
        Assert.AreEqual("1.1.x - 3.1.x", VersionRange.Parse("(1.0,3.1]").SimplePrint());

        Assert.AreEqual("1.1.x - 2.x", VersionRange.Parse("(1.0,3.0)").SimplePrint());
        Assert.AreEqual("1.x - 2.x", VersionRange.Parse("[1.0,3.0)").SimplePrint());
        Assert.AreEqual("1.x - 3.x", VersionRange.Parse("[1.0,3.0]").SimplePrint());
        Assert.AreEqual("1.1.x - 3.x", VersionRange.Parse("(1.0,3.0]").SimplePrint());

        Assert.AreEqual("1.1.x", VersionRange.Parse("(1.0,2.0)").SimplePrint());
        Assert.AreEqual("1.x", VersionRange.Parse("[1.0,2.0)").SimplePrint());
        Assert.AreEqual("1.x - 2.x", VersionRange.Parse("[1.0,2.0]").SimplePrint());
        Assert.AreEqual("1.1.x - 2.x", VersionRange.Parse("(1.0,2.0]").SimplePrint());

        Assert.AreEqual("1-pre - N", VersionRange.Parse("[1.0-pre,)").SimplePrint());
        Assert.AreEqual("1-alpha - N", VersionRange.Parse("[1.0-alpha,)").SimplePrint());

        Assert.AreEqual("1.1.x - 2.0.x", VersionRange.Parse("(1.0,2.1)").SimplePrint());
        Assert.AreEqual("1.x - 2.0.x", VersionRange.Parse("[1.0,2.1)").SimplePrint());
        Assert.AreEqual("1.x - 2.1.x", VersionRange.Parse("[1.0,2.1]").SimplePrint());
        Assert.AreEqual("1.1.x - 2.1.x", VersionRange.Parse("(1.0,2.1]").SimplePrint());

        Assert.AreEqual("1.1.x - N", VersionRange.Parse("(1.0,]").SimplePrint());
        Assert.AreEqual("1.x - N", VersionRange.Parse("[1.0,]").SimplePrint());
        Assert.AreEqual("N - 2.0.x", VersionRange.Parse("[,2.1)").SimplePrint());
        Assert.AreEqual("N - 2.1.x", VersionRange.Parse("[,2.1]").SimplePrint());


    }
}