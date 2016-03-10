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
        string suffix1;
        string suffix2;

        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void ShouldThrowForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => SnippetExtractor.IsStartCode("<!-- startcode -->", out fake, out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '<!-- startcode -->'.", exception.Message);
    }

    [Test]
    public void ShouldThrowForNoKeyWithNoSpace()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => SnippetExtractor.IsStartCode("<!--startcode-->", out fake, out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '<!--startcode-->'.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithsuffix1()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey 5 -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithSuffix1AndSuffix2()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey [1.0,2.0] package1 -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("[1.0,2.0]", suffix1);
        Assert.AreEqual("package1", suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!--startcode CodeKey-->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpacesWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!--startcode CodeKey 5-->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!--  startcode  CodeKey  -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!--  startcode  CodeKey  v5  -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode CodeKey 5", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code_Key -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code_Key 5 -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithDashes()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code-Key -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("<!-- startcode Code-Key 5 -->", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            SnippetExtractor.IsStartCode("<!-- startcode _key 6 -->", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            SnippetExtractor.IsStartCode("<!-- startcode key_ 6 -->", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            SnippetExtractor.IsStartCode("<!-- startcode _key-->", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            SnippetExtractor.IsStartCode("<!-- startcode key_ -->", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void CanExtractWithDifferentEndComments()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("/* startcode CodeKey 5 */", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        string key;
        string suffix1;
        string suffix2;
        var isStartCode = SnippetExtractor.IsStartCode("/*startcode CodeKey 5*/", out key, out suffix1, out suffix2);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

}