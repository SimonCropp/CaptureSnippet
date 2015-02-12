using System.IO;
using System.Linq;
using ApprovalTests;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetExtractorTests
{
    [Test]
    public void Duplicate_Key()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
    }


    [Test]
    public void Duplicate_Key_and_Version_and_language()
    {
        var input = @"
  <!-- startcode CodeKey 2-->
  <configSections/>
  <!-- endcode -->
  <!-- startcode CodeKey 2-->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
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
        Approvals.Verify(snippets);
    }

    [Test]
    public void SingleCodeQuoteDetected()
    {
        var input = @"
  <!-- startcode CodeKey-->
  sjfnskdjnf`knjknjkn`
  <!-- endcode -->";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
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
            var extractor = new SnippetExtractor(s => new Version(1, 1));
            var readSnippets = extractor.FromReader(stringReader);
            ObjectApprover.VerifyWithJson(readSnippets);
        }
    }

    public ReadSnippets FromText(string contents)
    {
        using (var stringReader = new StringReader(contents))
        {
            return new SnippetExtractor().FromReader(stringReader);
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
    public void UnClosedSnippet()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
    }

    [Test]
    public void UnClosedSnippetWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
    }

    [Test]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
    }

    [Test]
    public void UnClosedRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  <configSections/>";
        var snippets = FromText(input);
        Approvals.Verify(snippets.Errors.Single());
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