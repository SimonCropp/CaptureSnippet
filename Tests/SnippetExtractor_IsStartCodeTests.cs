using System;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class SnippetExtractor_IsStartCodeTests
{

    [Test]
    public void CanExtractFromXml()
    {
        string key;
        string version;

        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void ShouldThrowForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => SnippetExtractor.IsStartCode("<!-- startcode -->", out fake, out fake));
        Assert.AreEqual("No Key could be derived.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey 5 -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractFromXmlWithVersionRange()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey [1.0,2.0] -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("[1.0,2.0]", version);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!--startcode CodeKey-->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey",key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpacesWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!--startcode CodeKey 5-->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!--  startcode  CodeKey  -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!--  startcode  CodeKey  v5  -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", version);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey 5", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code_Key -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code_Key 5 -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithUnderScoresOutside()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode _CodeKey_ -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithUnderScoresOutsideWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode _CodeKey_ 5 -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithDashes()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code-Key -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code-Key 5 -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", version);
    }

    [Test]
    public void CanExtractWithDashesOutside()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode -CodeKey- -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(version);
    }

    [Test]
    public void CanExtractWithDashesOutsideWithVersion()
    {
        string key;
        string version;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode -CodeKey- 5 -->", out key, out version);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", version);
    }

}