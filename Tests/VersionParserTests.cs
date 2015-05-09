using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class VersionParserTests
{
    [Test]
    public void Simple()
    {
        Version version;

        Assert.IsTrue(VersionParser.TryParseVersion("1.2.3", out version));
        Assert.AreEqual("1.2.3", version.ToString());

        Assert.IsTrue(VersionParser.TryParseVersion("1.2.3", out version));
        Assert.AreEqual("1.2.3", version.ToString());

        Assert.IsTrue(VersionParser.TryParseVersion("1.2", out version));
        Assert.AreEqual("1.2", version.ToString());

        Assert.IsTrue(VersionParser.TryParseVersion("1.2", out version));
        Assert.AreEqual("1.2", version.ToString());
        
        Assert.IsTrue(VersionParser.TryParseVersion("1", out version));
        Assert.AreEqual("1", version.ToString());
        
        Assert.IsTrue(VersionParser.TryParseVersion("1", out version));
        Assert.AreEqual("1", version.ToString());

        Assert.IsTrue(VersionParser.TryParseVersion("noversion", out version));
        Assert.AreEqual(Version.ExplicitEmpty, version);

    }
}