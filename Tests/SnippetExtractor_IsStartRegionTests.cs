using System;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class SnippetExtractor_IsStartRegionTests
{

    [Test]
    public void CanExtractFromXml()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region CodeKey", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void SkouldIgnoreForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => SnippetExtractor.IsStartRegion("#region ", out fake, out fake));
        Assert.AreEqual("No Key could be derived.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region CodeKey 5", out key, out version);
        Assert.AreEqual("CodeKey",key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region  CodeKey   ", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region  CodeKey  v5    ", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", version);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region CodeKey", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }


    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region Code_Key", out key, out version);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region Code_Key 5", out key, out version);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithUnderScoresOutside()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region _CodeKey_", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithUnderScoresOutsideWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region _CodeKey_ v5", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", version);
    }

    [Test]
    public void CanExtractWithDashes()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region Code-Key", out key, out version);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region Code-Key 5", out key, out version);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithDashesOutside()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region -CodeKey-", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithDashesOutsideWithVersion()
    {
        string key;
        string version;
        SnippetExtractor.IsStartRegion("#region -CodeKey- v5", out key, out version);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", version);
    }

}