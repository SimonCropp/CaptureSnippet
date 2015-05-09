using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;
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
                version: new Version(1, 2),
                value: "1",
                startLine: 1,
                endLine: 1,
                file: null, 
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey1",
                version: new Version(1, 4),
                value: "2",
                startLine: 1,
                endLine: 1,
                file: null, 
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                version: new Version(1, 3),
                value: "3",
                startLine: 1,
                endLine: 1,
                file: null,
                language: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "cs",
                version: new Version(1, 4),
                value: "4",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "vb",
                version: new Version(1, 4),
                value: "4",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
            new ReadSnippet(
                key: "FoundKey2",
                language: "cs",
                version: new Version(1, 6),
                value: "5",
                startLine: 1,
                endLine: 1,
                file: string.Empty),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }
}