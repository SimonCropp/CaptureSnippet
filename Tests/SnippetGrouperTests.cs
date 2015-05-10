using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetGrouperTests
{
    [Test]
    public void Simple()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(key: "FoundKey1",
                version: new VersionRange(new SemanticVersion(1,2,0) ),
                value: "1",
                startLine: 1,
                endLine: 1,
                file: null, 
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey1",
                version: new VersionRange(new SemanticVersion(1, 4,0)),
                value: "2",
                startLine: 1,
                endLine: 1,
                file: null, 
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                version: new VersionRange(new SemanticVersion(1, 3,0)),
                value: "3",
                startLine: 1,
                endLine: 1,
                file: null,
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "cs",
                version: new VersionRange(new SemanticVersion(1, 4,0)),
                value: "4",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "vb",
                version: new VersionRange(new SemanticVersion(1, 4,0)),
                value: "4",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "cs",
                version: new VersionRange(new SemanticVersion(1, 6,0)),
                value: "5",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }
}