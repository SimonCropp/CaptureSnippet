using System;
using Xunit;

public class StartEndTester_IsStartCodeTests : TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => StartEndTester.IsStartCode("<!-- startcode -->", out fake, out fake));
        Assert.Equal("No Key could be derived. Line: '<!-- startcode -->'.", exception.Message);
    }

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        string fake;
        StartEndTester.IsStartCode("<!--startcode-->", out fake, out fake);
    }

    [Fact]
    public void CanExtractFromXmlWithSuffix()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5 -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey-->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpacesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey 5-->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpacesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  v5  -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("v5", suffix);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey 5", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractWithUnderScoresWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key 5 -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractWithDashesWithVersion()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key 5 -->", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key 6 -->", out fake, out fake));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ 6 -->", out fake, out fake));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key-->", out fake, out fake));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ -->", out fake, out fake));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isStartCode = StartEndTester.IsStartCode("/* startcode CodeKey 5 */", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("/*startcode CodeKey 5*/", out var key, out var suffix);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }
}