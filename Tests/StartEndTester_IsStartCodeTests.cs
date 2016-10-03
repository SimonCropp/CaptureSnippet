using System;
using ApprovalTests.Reporters;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class StartEndTester_IsStartCodeTests
{

    [Test]
    public void CanExtractFromXml()
    {
        string key;
        string suffix;

        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void ShouldThrowForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => StartEndTester.IsStartCode("<!-- startcode -->", out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '<!-- startcode -->'.", exception.Message);
    }

    [Test]
    public void ShouldThrowForNoKeyWithNoSpace()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => StartEndTester.IsStartCode("<!--startcode-->", out fake, out fake));
        Assert.AreEqual("No Key could be derived. Line: '<!--startcode-->'.", exception.Message);
    }

    [Test]
    public void CanExtractFromXmlWithSuffix()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5 -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey-->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpacesWithVersion()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey 5-->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  v5  -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key 5 -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithDashes()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key 5 -->", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key 6 -->", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ 6 -->", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key-->", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Test]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ -->", out fake, out fake));
        Assert.AreEqual("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Test]
    public void CanExtractWithDifferentEndComments()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("/* startcode CodeKey 5 */", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        string key;
        string suffix;
        var isStartCode = StartEndTester.IsStartCode("/*startcode CodeKey 5*/", out key, out suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

}