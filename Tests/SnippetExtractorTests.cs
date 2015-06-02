using System.IO;
using System.Threading.Tasks;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetExtractorTests
{
    [Test]
    public async void WithDodgyEmDash()
    {
        var input = @"
  <!-- startcode key -->
  —
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }
    [Test]
    public async void WithDodgyLeftQuote()
    {
        var input = @"
  <!-- startcode key -->
  “
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void WithDodgyRightQuote()
    {
        var input = @"
  <!-- startcode key -->
  ”
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void WithCodeTick()
    {
        var input = @"
  <!-- startcode key -->
  `
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void Differ_by_version_missing_suffix()
    {
        var input = @"
  <!-- startcode CodeKey 2-->
  <configSections/>
  <!-- endcode -->
  <!-- startcode CodeKey 2.0-->
  <configSections/>
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void SingleCodeQuoteDetected()
    {
        var input = @"
  <!-- startcode CodeKey-->
  sjfnskdjnf`knjknjkn`
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void CanExtractMultipleWithDifferentVersions()
    {
        var input = @"
  <!-- startcode CodeKey 4 -->
  <configSections/>
  <!-- endcode -->
  #region CodeKey 5
  The Code
  #endregion";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractFromXml()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractVersionFromFile()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        using (var stringReader = new StringReader(input))
        {
            var versionRange = new VersionRange(new SemanticVersion(1, 1, 0));
            var extractor = new SnippetExtractor(s => versionRange);
            var readSnippets = await extractor.FromReader(stringReader);
            ObjectApprover.VerifyWithJson(readSnippets);
        }
    }

    public async Task<ReadSnippets> FromText(string contents)
    {
        using (var stringReader = new StringReader(contents))
        {
            var extractor = new SnippetExtractor(s => VersionRange.All);
            return await extractor.FromReader(stringReader);
        }
    }

    [Test]
    public async void CanExtractFromXmlWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractFromXmlWithVersionRange()
    {
        var input = @"
  <!-- startcode CodeKey [1.0,2.0] -->
  <configSections/>
  <!-- endcode -->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void UnClosedSnippet()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void UnClosedSnippetWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void UnClosedRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  <configSections/>";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public async void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractFromRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  The Code
  #endregion";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode ";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithNoTrailingCharactersWithVersion()
    {
        var input = @"
  // startcode CodeKey 6
  the code
  // endcode ";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--startcode CodeKey-->
  <configSections/>
  <!--endcode-->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithMissingSpacesWithVersion()
    {
        var input = @"
  <!--startcode CodeKey 6-->
  <configSections/>
  <!--endcode-->";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode   ";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public async void CanExtractWithTrailingWhitespaceWithVersion()
    {
        var input = @"
  // startcode CodeKey 4
  the code
  // endcode   ";
        var snippets = await FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }
}