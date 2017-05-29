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
public class CSharpNamespaceExtractorTests
{
    [Test]
    public void CanExtractUsingsFromCSharpLanguage()
    {
        var input = @"
  #region UsingsSection
  using System;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void WithNameSpaceThatHasKeywordInIt()
    {
        var input = @"
  #region UsingsSection
  using LibraryUsingSystem;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
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

    [Test]
    public void WithNamespaceThatIncludesAlias()
    {
        var input = @"
  #region UsingsSection
using Math = System.Math;
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
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

    [Test]
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

    public List<Snippet> FromText(string contents)
    {
        var extractor = FileSnippetExtractor.Build(VersionRange.All, "package", false);
        using (var stringReader = new StringReader(contents))
        {
            return extractor.AppendFromReader(stringReader, "path.cs").ToList();
        }
    }

}
