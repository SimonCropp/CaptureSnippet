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
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region CodeKey", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => 
        SnippetExtractor.IsStartRegion("#region _key 6", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => 
        SnippetExtractor.IsStartRegion("#region key_ 6", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }
    [Test]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
        SnippetExtractor.IsStartRegion("#region _key", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
        SnippetExtractor.IsStartRegion("#region key_ ", out fake, out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols.", exception.Message);
    }


    [Test]
    public void ShouldIgnoreForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
        SnippetExtractor.IsStartRegion("#region ", out fake, out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithVersion()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region CodeKey 5", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey",key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region  CodeKey   ", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithSuffix1()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region  CodeKey  v5    ", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix1);
        Assert.IsNull(suffix2);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithSuffix1AndSuffix2()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region  CodeKey  v5  package1  ", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix1);
        Assert.AreEqual("package1", suffix2);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region CodeKey", out key, out suffix1, out suffix2);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix1);
        Assert.IsNull(suffix2);
    }


    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        string suffix1;
        string suffix2;
        SnippetExtractor.IsStartRegion("#region Code_Key", out key, out suffix1, out suffix2);
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
        SnippetExtractor.IsStartRegion("#region Code_Key 5", out key, out suffix1, out suffix2);
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
        SnippetExtractor.IsStartRegion("#region Code-Key", out key, out suffix1, out suffix2);
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
        SnippetExtractor.IsStartRegion("#region Code-Key 5", out key, out suffix1, out suffix2);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", suffix1);
        Assert.IsNull(suffix2);
    }


}