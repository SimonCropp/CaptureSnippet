using System;
using ApprovalTests.Reporters;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class StartEndTester_IsStartRegionTests
{

    [Test]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key, out var suffix);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region _key 6", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region key_ 6", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region _key", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region key_ ", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: key_", exception.Message);
    }


    [Test]
    public void ShouldIgnoreForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region ", out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithVersion()
    {
        StartEndTester.IsStartRegion("#region CodeKey 5", out var key, out var suffix);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var key, out var suffix);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithSuffix1()
    {
        StartEndTester.IsStartRegion("#region  CodeKey  v5    ", out var key, out var suffix);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix);
    }


    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key, out var suffix);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }


    [Test]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var key, out var suffix);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        StartEndTester.IsStartRegion("#region Code_Key 5", out var key, out var suffix);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", suffix);
    }


    [Test]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var key, out var suffix);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        StartEndTester.IsStartRegion("#region Code-Key 5", out var key, out var suffix);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", suffix);
    }
}