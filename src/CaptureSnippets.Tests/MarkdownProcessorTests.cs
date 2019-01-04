using System.Collections.Generic;
using CaptureSnippets;
using NuGet.Versioning;
using Xunit;

public class MarkdownProcessorTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild(
                language: "cs",
                key: "snippet1",
                package: "package1",
                version: CreateVersionRange(5)
            ),
            SnippetBuild(
                language: "cs",
                key: "snippet1",
                package: "package1",
                version: CreateVersionRange(4)
            ),
            SnippetBuild(
                language: "cs",
                key: "snippet2",
                package: "package1",
                version: CreateVersionRange(3)
            ),
            SnippetBuild(
                language: "cs",
                key: "snippet2",
                package: "package1",
                version: CreateVersionRange(4)),
        };
        var markdownContent = @"
snippet: snippet1

some text

snippet: snippet2

some other text

";
        SnippetVerifier.Verify(markdownContent, availableSnippets);
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

    static VersionRange CreateVersionRange(int version)
    {
        return new VersionRange(minVersion: new NuGetVersion(version, 0, 0));
    }
}