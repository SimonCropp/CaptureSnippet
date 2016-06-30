using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
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
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void WithDodgyLeftQuote()
    {
        var input = @"
  <!-- startcode key -->
  “
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void WithDodgyRightQuote()
    {
        var input = @"
  <!-- startcode key -->
  ”
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void WithCodeTick()
    {
        var input = @"
  <!-- startcode key -->
  `
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
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
  foo`bar`
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
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
            var versionRange = new VersionRange(new NuGetVersion(1, 1, 0));
            var snippets = new List<ReadSnippet>();
            var result = PathData.With(versionRange, Package.Undefined, Component.Undefined);
            var extractor = new FileSnippetExtractor(y => result);
            extractor.AppendFromReader(stringReader, "path.cs", versionRange, Package.Undefined, Component.Undefined, snippets.Add)
                .GetAwaiter()
                .GetResult();
            ObjectApprover.VerifyWithJson(snippets.Single());
        }
    }

    public List<ReadSnippet> FromText(string contents, ExtractFileNameData extractFileNameData = null)
    {
        using (var stringReader = new StringReader(contents))
        {
            var snippets = new List<ReadSnippet>();
            var result = PathData.With(VersionRange.All, Package.Undefined, Component.Undefined);
            if (extractFileNameData == null)
            {
                extractFileNameData = y => result;
            }
            var extractor = new FileSnippetExtractor(extractFileNameData);
            extractor.AppendFromReader(stringReader, "path.cs", VersionRange.All, Package.Undefined, Component.Undefined, snippets.Add).GetAwaiter().GetResult();
            return snippets;
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
    public void CanExtractFromWithVersionAndPackage1()
    {
        var input = @"
  #region CodeKey 5 package1
  The code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromWithVersionAndPackage2()
    {
        var input = @"
  #region CodeKey package1 5
  The code
  #endregion";
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
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void UnClosedSnippetWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void UnClosedRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
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