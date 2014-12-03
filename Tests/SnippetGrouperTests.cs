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
                           new ReadSnippet
                           {
                               Key = "FoundKey1",
                               Version = new Version(1,2),
                               Value = "1"
                           },
                           new ReadSnippet
                           {
                               Key = "FoundKey1",
                               Version = new Version(1,4),
                               Value = "2"
                           },
                           new ReadSnippet
                           {
                               Key = "FoundKey2",
                               Version = new Version(1,3),
                               Value = "3"
                           },
                           new ReadSnippet
                           {
                               Key = "FoundKey2",
                               Language = "cs",
                               Version = new Version(1,4),
                               Value = "4"
                           },
                           new ReadSnippet
                           {
                               Key = "FoundKey2",
                               Language = "vb",
                               Version = new Version(1,4),
                               Value = "4"
                           },
                           new ReadSnippet
                           {
                               Key = "FoundKey2",
                               Language = "cs",
                               Version = new Version(1,6),
                               Value = "5"
                           },
                       };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }
}