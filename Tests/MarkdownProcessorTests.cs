using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        var availableSnippets = new List<SnippetGroup>
        {
            new SnippetGroup(
                language: "cs",
                component:Component.Undefined,
                key: "versionedSnippet1",
                packages: new List<PackageGroup>
                {
                    new PackageGroup(
                        package: "package1",
                        versions: new List<VersionGroup>
                        {
                            CreateVersionGroup(5),
                            CreateVersionGroup(4),
                        })
                }),
            new SnippetGroup(
                language: "cs",
                component:Component.Undefined, 
                key: "versionedSnippet2",
                packages: new List<PackageGroup>
                {
                    new PackageGroup(
                        package: "package1",
                        versions: new List<VersionGroup>
                        {
                            CreateVersionGroup(3),
                            CreateVersionGroup(2),
                        })
                }),
            new SnippetGroup(
                language: "cs",
                component:Component.Undefined,
                key: "nonVersionedSnippet1",
                packages: new List<PackageGroup>
                {
                    new PackageGroup(
                        package: "package1",
                        versions: new List<VersionGroup>
                        {
                            CreateVersionGroup(5),
                        })
                }),
            new SnippetGroup(
                language: "cs",
                component:Component.Undefined,
                key: "nonVersionedSnippet2",
                packages: new List<PackageGroup>
                {
                    new PackageGroup(
                        package: "package1",
                        versions: new List<VersionGroup>
                        {
                            CreateVersionGroup(5),
                        })
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
        var markdownProcessor = new MarkdownProcessor(
            snippets: availableSnippets,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup);
        var stringBuilder = new StringBuilder();
        using (var reader = new StringReader(markdownContent))
        using (var writer = new StringWriter(stringBuilder))
        {
            var processResult = markdownProcessor.Apply(reader, writer)
                .Result;
            var output = new object[]
            {
                processResult.MissingSnippets, processResult.UsedSnippets, stringBuilder.ToString()
            };
            ObjectApprover.VerifyWithJson(output, s => s.Replace("\\r\\n", "\r\n"));
        }
    }


    static VersionGroup CreateVersionGroup(int version)
    {
        var versionRange = new VersionRange(minVersion: new NuGetVersion(version, 0, 0));
        return new VersionGroup(
            version: versionRange,
            value: "Snippet_v" + version,
            sources: new List<SnippetSource>
            {
                new SnippetSource(
                    version: versionRange,
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
                component:Component.Undefined,
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                component:Component.Undefined,
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        Verify("snippet: MissingKey", snippetGroups);
    }

    [Test]
    public void MissingMultipleKeys()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                component:Component.Undefined,
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                component:Component.Undefined,
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        Verify("snippet: MissingKey1\r\n\r\nsnippet: MissingKey2", snippetGroups);
    }


    [Test]
    public void LotsOfText()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                component:Component.Undefined,
                value: "Value1",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                component:Component.Undefined,
                value: "Value2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey3",
                component:Component.Undefined,
                value: "Value3",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey4",
                component:Component.Undefined,
                value: "Value4",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
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
snippet: FoundKey3
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
";
        Verify(markdownContent, snippetGroups);
    }
}