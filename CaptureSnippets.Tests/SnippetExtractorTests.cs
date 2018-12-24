﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;
using ObjectApproval;
using Xunit;

public class SnippetExtractorTests : TestBase
{
    [Fact]
    public void WithDodgyEmDash()
    {
        var input = @"
  <!-- startcode key -->
  —
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithDodgyLeftQuote()
    {
        var input = @"
  <!-- startcode key -->
  “
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithDodgyRightQuote()
    {
        var input = @"
  <!-- startcode key -->
  ”
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }


    [Fact]
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



    [Fact]
    public void CanExtractWithInnerWhiteSpace()
    {
        var input = @"
  #region CodeKey 5

  BeforeWhiteSpace

  AfterWhiteSpace

  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedBroken()
    {
        var input = @"
  #region KeyParent
  a
  #region KeyChild
  b
  c
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedRegion()
    {
        var input = @"
  #region KeyParent
  a
  #region KeyChild
  b
  #endregion
  c
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedMixed2()
    {
        var input = @"
  #region KeyParent
  a
  <!-- startcode KeyChild -->
  b
  <!-- endcode -->
  c
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedStartCode()
    {
        var input = @"
  <!-- startcode KeyParent -->
  a
  <!-- startcode KeyChild -->
  b
  <!-- endcode -->
  c
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedMixed1()
    {
        var input = @"
  <!-- startcode KeyParent -->
  a
  #region KeyChild
  b
  #endregion
  c
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
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


    [Fact]
    public void CanExtractFromXml()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractVersionFromFile()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        using (var stringReader = new StringReader(input))
        {
            var versionRange = new VersionRange(new NuGetVersion(1, 1, 0));
            var extractor = FileSnippetExtractor.Build(versionRange, "package", false);
            var snippets = extractor.Read(stringReader, "path.cs").ToList();
            ObjectApprover.VerifyWithJson(snippets.Single());
        }
    }

    public List<Snippet> FromText(string contents)
    {
        var extractor = FileSnippetExtractor.Build(VersionRange.All, "package", false);
        using (var stringReader = new StringReader(contents))
        {
            return extractor.Read(stringReader, "path.cs").ToList();
        }
    }

    [Fact]
    public void CanExtractFromXmlWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromAllVersion()
    {
        var input = @"
  <!-- startcode CodeKey 1 -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromXmlWithVersionRange()
    {
        var input = @"
  <!-- startcode CodeKey [1.0,2.0] -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void UnClosedSnippet()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void UnClosedSnippetWithVersion()
    {
        var input = @"
  <!-- startcode CodeKey 5 -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void UnClosedRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromRegionWithVersion()
    {
        var input = @"
  #region CodeKey 5
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharactersWithVersion()
    {
        var input = @"
  // startcode CodeKey 6
  the code
  // endcode ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--startcode CodeKey-->
  <configSections/>
  <!--endcode-->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithMissingSpacesWithVersion()
    {
        var input = @"
  <!--startcode CodeKey 6-->
  <configSections/>
  <!--endcode-->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode   ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
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