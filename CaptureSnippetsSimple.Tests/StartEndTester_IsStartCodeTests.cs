using System;
using Xunit;

public class StartEndTester_IsStartCodeTests : TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() => StartEndTester.IsStartCode("<!-- startcode -->", out fake));
        Assert.Equal("No Key could be derived. Line: '<!-- startcode -->'.", exception.Message);
    }

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        string fake;
        StartEndTester.IsStartCode("<!--startcode-->", out fake);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey-->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key-->", out fake));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ -->", out fake));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isStartCode = StartEndTester.IsStartCode("/* startcode CodeKey */", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("/*startcode CodeKey */", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }
}