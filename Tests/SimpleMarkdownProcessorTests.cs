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
public class SimpleMarkdownProcessorTests
{

    [Test]
    public void Simple()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "snippet2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: "snippet2Value",
                language: "c",
                path: "unknown",
                package: "package1"),
            new ReadSnippet(
                key: "snippet1",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: "snippet1Value",
                language: "c",
                path: "unknown",
                package: "package1"),
        };
        var markdownContent = @"
snippet: snippet1

some text

snippet: snippet2

some other text

";
        Verify(markdownContent, snippets);
    }

    static void Verify(string markdownContent, List<ReadSnippet> availableSnippets)
    {
        var processor = new SimpleMarkdownProcessor();
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
                path: "unknown",
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
        };
        Verify("snippet: MissingKey", snippets);
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
                path: "unknown",
                package: "package1"),
            new ReadSnippet(key: "foundkey2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                value: " ",
                language: "c",
                path: "unknown",
                package: "package1"),
        };
        Verify("snippet: MissingKey1\r\n\r\nsnippet: MissingKey2", snippets);
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
                path: null,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                value: "Value2",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
            new ReadSnippet(key: "foundkey3",
                value: "Value3",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
            new ReadSnippet(key: "foundkey4",
                value: "Value4",
                version: VersionRange.All,
                startLine: 1,
                endLine: 1,
                language: "c",
                path: null,
                package: "package1"),
        };
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
        Verify(markdownContent, snippets);
    }
}