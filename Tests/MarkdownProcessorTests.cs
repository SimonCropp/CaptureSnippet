using System.Collections.Generic;
using System.IO;
using System.Text;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;
// ReSharper disable StringLiteralTypo

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class MarkdownProcessorTests
{

    [Test]
    public void Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild(
                language: "cs",
                key: "snippet1",
                package: "package1",
                version:CreateVersionRange(5)
                ),
            SnippetBuild(
                language: "cs",
                key: "snippet1",
                package: "package1",
                version:CreateVersionRange(4)
                ),
            SnippetBuild(
                language: "cs",
                key: "snippet2",
                package: "package1",
                version:CreateVersionRange(3)
                ),
            SnippetBuild(
                language: "cs",
                key: "snippet2",
                package: "package1",
                version:CreateVersionRange(4)),
        };
        var markdownContent = @"
snippet: snippet1

some text

snippet: snippet2

some other text

";
        Verify(markdownContent, availableSnippets.ToDictionary());
    }

    Snippet SnippetBuild(string language, string key, string package, VersionRange version)
    {
        return Snippet.Build(
            language: language,
            startLine: 1,
            endLine: 2,
            value: "Snippet_v" + version,
            key: key,
            path: "thePath",
            version: version,
            package: package,
            isCurrent: false,
            includes: null);
    }

    static void Verify(string markdownContent, IReadOnlyDictionary<string, IReadOnlyList<Snippet>> availableSnippets)
    {
        var markdownProcessor = new MarkdownProcessor(
            snippets: availableSnippets,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup);
        var stringBuilder = new StringBuilder();
        using (var reader = new StringReader(markdownContent))
        using (var writer = new StringWriter(stringBuilder))
        {
            var processResult = markdownProcessor.Apply(reader, writer);
            var output = new
            {
                processResult.MissingSnippets,
                processResult.UsedSnippets,
                content= stringBuilder.ToString()
            };
            ObjectApprover.VerifyWithJson(output, s => s.Replace("\\r\\n", "\r\n"));
        }
    }


    static VersionRange CreateVersionRange(int version)
    {
        return new VersionRange(minVersion: new NuGetVersion(version, 0, 0));
    }

    //    [Test]
    //    public void MissingKey()
    //    {
    //        var snippets = new List<Snippet>
    //        {
    //            Snippet.Build(
    //                key: "foundkey1",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                value: " ",
    //                language: "c",
    //                path: "unknown",
    //                package: "package1"),
    //            Snippet.Build(
    //                key: "foundkey2",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                value: " ",
    //                language: "c",
    //                path: "unknown",
    //                package: "package1"),
    //        };
    //        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
    //        Verify("snippet: MissingKey", snippetGroups);
    //    }

    //    [Test]
    //    public void MissingMultipleKeys()
    //    {
    //        var snippets = new List<ReadSnippet>
    //        {
    //            new ReadSnippet(
    //                key: "foundkey1",
    //                component:Component.Undefined,
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                value: " ",
    //                language: "c",
    //                path: "unknown",
    //                package: "package1"),
    //            new ReadSnippet(
    //                key: "foundkey2",
    //                component:Component.Undefined,
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                value: " ",
    //                language: "c",
    //                path: "unknown",
    //                package: "package1"),
    //        };
    //        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
    //        Verify("snippet: MissingKey1\r\n\r\nsnippet: MissingKey2", snippetGroups);
    //    }


    //    [Test]
    //    public void LotsOfText()
    //    {
    //        var snippets = new List<ReadSnippet>
    //        {
    //            new ReadSnippet(
    //                key: "foundkey1",
    //                component:Component.Undefined,
    //                value: "Value1",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                language: "c",
    //                path: null,
    //                package: "package1"),
    //            new ReadSnippet(
    //                key: "foundkey2",
    //                component:Component.Undefined,
    //                value: "Value2",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                language: "c",
    //                path: null,
    //                package: "package1"),
    //            new ReadSnippet(
    //                key: "foundkey3",
    //                component:Component.Undefined,
    //                value: "Value3",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                language: "c",
    //                path: null,
    //                package: "package1"),
    //            new ReadSnippet(
    //                key: "foundkey4",
    //                component:Component.Undefined,
    //                value: "Value4",
    //                version: VersionRange.All,
    //                startLine: 1,
    //                endLine: 1,
    //                language: "c",
    //                path: null,
    //                package: "package1"),
    //        };
    //        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
    //        var markdownContent = @"
    //snippet: FoundKey2
    //snippet: FoundKey1
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmg
    //kdjrngkjfncgdflkgmxdklfmgkdflxmg
    //kdjrngkjfncgdflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //snippet: FoundKey3
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmg
    //kdjrngkjfncgdflkgmxdklfmgkdflxmg
    //kdjrngkjfncgdflkgmxdklfmgkdflxmg
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //dflkgmxdklfmgkdflxmgfkgjnfdjkgn
    //dflkgmxdklfmgkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //dflkgmxdklfmdfgkjndfkjgngkdflxmg
    //";
    //        Verify(markdownContent, snippetGroups);
    //    }
}