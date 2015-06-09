using System.IO;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetExtractorTests
{
    [Test]
    public void WithDodgyEmDash()
    {
        var input = @"
  <!-- startcode key -->
  —
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }
    [Test]
    public void WithDodgyLeftQuote()
    {
        var input = @"
  <!-- startcode key -->
  “
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void WithDodgyRightQuote()
    {
        var input = @"
  <!-- startcode key -->
  ”
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void WithCodeTick()
    {
        var input = @"
  <!-- startcode key -->
  `
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void Differ_by_version_missing_suffix()
    {
        var input = @"
  <!-- startcode CodeKey 2-->
  <configSections/>
  <!-- endcode -->
  <!-- startcode CodeKey 2.0-->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void SingleCodeQuoteDetected()
    {
        var input = @"
  <!-- startcode CodeKey-->
  sjfnskdjnf`knjknjkn`
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void CanExtractMultipleWithDifferentVersions()
    {
        var input = @"
  <!-- startcode CodeKey 4 -->
  <configSections/>
  <!-- endcode -->
  #region CodeKey 5
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromXml()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractVersionFromFile()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        using (var stringReader = new StringReader(input))
        {
            var versionRange = new VersionRange(new SemanticVersion(1, 1, 0));
            var extractor = new SnippetExtractor(s => versionRange);
            var readSnippets = extractor.FromReader(stringReader);
            ObjectApprover.VerifyWithJson(readSnippets);
        }
    }

    public ReadSnippets FromText(string contents)
    {
        using (var stringReader = new StringReader(contents))
        {
            var extractor = new SnippetExtractor(s => VersionRange.All);
            return extractor.FromReader(stringReader);
        }
    }

    [Test]
    public void CanExtractFromXmlWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }
    [Test]
    public void CanExtractFromAllVersion()
    {
        var input = @"
  <!-- startcode CodeKey All -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromXmlWithVersionRange()
    {
        var input = @"
  <!-- startcode CodeKey [1.0,2.0] -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void UnClosedSnippet()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void UnClosedSnippetWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void UnClosedRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets.Errors);
    }

    [Test]
    public void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        var input = @"
  // startcode CodeKey 6
  the code
  // endcode ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--startcode CodeKey-->
  <configSections/>
  <!--endcode-->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithMissingSpacesWithVersion()
    {
        var input = @"
  <!--startcode CodeKey 6-->
  <configSections/>
  <!--endcode-->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode   ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithTrailingWhitespaceWithVersion()
    {
        var input = @"
  // startcode CodeKey 4
  the code
  // endcode   ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }
}