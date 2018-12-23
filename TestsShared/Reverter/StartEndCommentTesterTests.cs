using System;
using Xunit;

public class StartEndCommentTesterTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isStartCode = StartEndCommentTester.IsStart("<!-- snippet: CodeKey -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        var exception = Assert.Throws<Exception>(() => StartEndCommentTester.IsStart("<!-- snippet: -->", out _));
        Assert.Equal("No Key could be derived. Line: '<!-- snippet: -->'.", exception.Message);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndCommentTester.IsStart("<!-- snippet: Code_Key -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndCommentTester.IsStart("<!-- snippet: Code-Key -->", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<Exception>(() =>
            StartEndCommentTester.IsStart("<!-- snippet: _key -->", out _));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<Exception>(() =>
            StartEndCommentTester.IsStart("<!-- snippet: key_ -->", out _));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }
}