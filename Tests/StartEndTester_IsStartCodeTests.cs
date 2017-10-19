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
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey -->", out var key, out var suffix);
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
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        string fake;
        StartEndTester.IsStartCode("<!--startcode-->", out fake, out fake);
    }

    [Test]
    public void CanExtractFromXmlWithSuffix()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5 -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey-->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpacesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey 5-->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  v5  -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("v5", suffix);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithUnderScoresWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key 5 -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code_Key", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key -->", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("Code-Key", key);
        Assert.IsNull(suffix);
    }

    [Test]
    public void CanExtractWithDashesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key 5 -->", out var key, out var suffix);
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
        var isStartCode = StartEndTester.IsStartCode("/* startcode CodeKey 5 */", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

    [Test]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("/*startcode CodeKey 5*/", out var key, out var suffix);
        Assert.IsTrue(isStartCode);
        Assert.AreEqual("CodeKey", key);
        Assert.AreEqual("5", suffix);
    }

}