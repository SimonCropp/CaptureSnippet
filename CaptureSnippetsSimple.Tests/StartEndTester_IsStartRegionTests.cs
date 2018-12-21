using System;
using Xunit;

public class StartEndTester_IsStartRegionTests : TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region _key", out fake));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region key_ ", out fake));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }


    [Fact]
    public void ShouldIgnoreForNoKey()
    {
        string fake;
        var exception = Assert.Throws<Exception>(() =>
            StartEndTester.IsStartRegion("#region ", out fake));
        Assert.Equal("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var key);
        Assert.Equal("CodeKey", key);
    }


    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("CodeKey", key);
    }


    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var key);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var key);
        Assert.Equal("Code-Key", key);
    }
}