using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;
// ReSharper disable StringLiteralTypo

[TestFixture]
public class MarkdownProcessorTests
{

    [Test]
    public void Simple()
    {
        var availableSnippets = new List<SnippetGroup>
        {
            new SnippetGroup(
                language:"cs",
                key: "versionedSnippet1",
                versions: new List<VersionGroup>
                {
                    CreateVersionGroup(5),
                    CreateVersionGroup(4),
                }),
            new SnippetGroup(
                language:"cs",
                key: "versionedSnippet2",
                versions: new List<VersionGroup>
                {
                    CreateVersionGroup(3),
                    CreateVersionGroup(2),
                }),
            new SnippetGroup(
                language:"cs",
                key: "nonVersionedSnippet1",
                versions: new List<VersionGroup>
                {
                    CreateVersionGroup(5),
                }),
            new SnippetGroup(
                language:"cs",
                key: "nonVersionedSnippet2",
                versions: new List<VersionGroup>
                {
                    CreateVersionGroup(5),
                }),
        };
        var markdownContent = @"
snippet: versionedSnippet1

some text

snippet: versionedSnippet2

some other text

snippet: nonVersionedSnippet1

even more text

snippet: nonVersionedSnippet2

";
        Verify(markdownContent, availableSnippets);
    }

    static void Verify(string markdownContent, List<SnippetGroup> availableSnippets)
    {
        var processor = new MarkdownProcessor();
        var stringBuilder = new StringBuilder();
        using (var reader = new StringReader(markdownContent))
        using (var writer = new StringWriter(stringBuilder))
        {
            var processResult = processor.Apply(availableSnippets, reader, writer).Result;
            var output = new object[]
            {
                processResult.MissingSnippets, processResult.UsedSnippets, stringBuilder.ToString()
            };
            ObjectApprover.VerifyWithJson(output, s => s.Replace("\\r\\n", "\r\n"));
        }
    }


    static VersionGroup CreateVersionGroup(int version)
    {
        return new VersionGroup(
            version: new VersionRange(minVersion: new NuGetVersion(version, 0, 0)),
            value: "Snippet_v" + version,
            sources: new List<SnippetSource>
            {
                new SnippetSource(
                    startLine: 1,
                    endLine: 2,
                    file: null
                    )
            });
    }

    [Test]
    public void MissingKey()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                file: "unknown"),
            new ReadSnippet(
                key: "foundkey2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                file: "unknown"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        Verify("snippet: MissingKey", snippetGroups);
    }

    [Test]
    public void MissingMultipleKeys()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(key: "foundkey1",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                file: "unknown"),
            new ReadSnippet(key: "foundkey2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                file: "unknown"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        Verify("snippet: MissingKey1\r\n\r\nsnippet: MissingKey2", snippetGroups);
    }


    [Test]
    public void LotsOfText()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(key: "foundkey1",
                value: "Value1",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                file: null),
            new ReadSnippet(
                key: "foundkey2",
                value: "Value2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                file: null),
            new ReadSnippet(key: "foundkey3",
                value: "Value3",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                file: null),
            new ReadSnippet(key: "foundkey4",
                value: "Value4",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                file: null),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        var markdownContent = @"
snippet: FoundKey2
snippet: FoundKey1
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
snippet: FoundKey1
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
snippet: FoundKey1
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
snippet: FoundKey1
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
snippet: FoundKey1
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
snippet: FoundKey1
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
snippet: FoundKey1
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
snippet: FoundKey1
snippet: FoundKey1
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
snippet: FoundKey1
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
lkmdflkgmxdklfmgkdflxmg
";
        Verify(markdownContent, snippetGroups);
    }
}