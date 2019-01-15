using CaptureSnippets;
using Xunit;

public class StartEndTester_IsStartRegionTests : TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key, out var suffix);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbolAndVersion()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region _key 6", out _, out _));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbolAndVersion()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region key_ 6", out _, out _));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region _key", out _, out _));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region key_ ", out _, out _));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }


    [Fact]
    public void ShouldIgnoreForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region ", out _, out _));
        Assert.Equal("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Fact]
    public void CanExtractFromXmlWithVersion()
    {
        StartEndTester.IsStartRegion("#region CodeKey 5", out var key, out var suffix);
        Assert.Equal("CodeKey", key);
        Assert.Equal("5", suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var key, out var suffix);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpacesWithSuffix1()
    {
        StartEndTester.IsStartRegion("#region  CodeKey  v5    ", out var key, out var suffix);
        Assert.Equal("CodeKey", key);
        Assert.Equal("v5", suffix);
    }


    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key, out var suffix);
        Assert.Equal("CodeKey", key);
        Assert.Null(suffix);
    }


    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var key, out var suffix);
        Assert.Equal("Code_Key", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractWithUnderScoresWithVersion()
    {
        StartEndTester.IsStartRegion("#region Code_Key 5", out var key, out var suffix);
        Assert.Equal("Code_Key", key);
        Assert.Equal("5", suffix);
    }


    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var key, out var suffix);
        Assert.Equal("Code-Key", key);
        Assert.Null(suffix);
    }

    [Fact]
    public void CanExtractWithDashesWithVersion()
    {
        StartEndTester.IsStartRegion("#region Code-Key 5", out var key, out var suffix);
        Assert.Equal("Code-Key", key);
        Assert.Equal("5", suffix);
    }
}