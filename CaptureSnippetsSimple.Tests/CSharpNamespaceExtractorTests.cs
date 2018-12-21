using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;
using ObjectApproval;
using Xunit;

public class CSharpNamespaceExtractorTests : TestBase
{
    [Fact]
    public void CanExtractUsingsFromCSharpLanguage()
    {
        var input = @"
  #region UsingsSection
  using System;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithNameSpaceThatHasKeywordInIt()
    {
        var input = @"
  #region UsingsSection
  using LibraryUsingSystem;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithSnippetIncludingKeywordInComments()
    {
        var input = @"
  #region UsingsSection
//using of System;
using System;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithNamespaceThatIncludesAlias()
    {
        var input = @"
  #region UsingsSection
using Math = System.Math;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithDuplicatedUsings()
    {
        var input = @"
  #region UsingsSection
using System;
using System.Core;
using System;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithIncludesOutOfSnippetRegion()
    {
        var input = @"
using System;
using System.Core;

  #region TestClass
  public class Test { }
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithOutOfOrderIncludes()
    {
        var input = @"
using System.Core;
using System;
using System.Xml.Linq;

  #region TestClass
  public class Test { }
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    List<Snippet> FromText(string contents)
    {
        var extractor = FileSnippetExtractor.Build("package");
        using (var stringReader = new StringReader(contents))
        {
            return extractor.AppendFromReader(stringReader, "path.cs").ToList();
        }
    }
}