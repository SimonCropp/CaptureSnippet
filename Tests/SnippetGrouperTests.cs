using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class SnippetGrouperTests
{

    [Test]
    public void Mixing_null_and_non_null_packages()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "key",
                version: VersionRange.All,
                value: "1",
                startLine: 1,
                endLine: 3,
                path: @"c:\files\file2.txt",
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "key",
                version:  VersionRange.All,
                value: "1",
                startLine: 4,
                endLine: 5,
                path: @"c:\files\file1.txt",
                language: string.Empty,
                package: Package.None),
        };
        var snippetGroups = SnippetGrouper.Group(snippets);
        var readSnippetError = snippetGroups.Errors.Single();
        Approvals.Verify(readSnippetError);
    }
    [Test]
    public void Mixing_null_and_non_null_versions()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "key",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "key",
                version:  VersionRange.All,
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets);
        var readSnippetError = snippetGroups.Errors.Single();
        Approvals.Verify(readSnippetError);
    }

    [Test]
    public void Duplicate_Key_different_package()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "packageA"),
            new ReadSnippet(
                key: "foundkey",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "packageB"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets);
        ObjectApprover.VerifyWithJson(snippetGroups.Groups);
    }

    [Test]
    public void Duplicate_Key()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "packageA"),
            new ReadSnippet(
                key: "foundkey",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "packageA"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets);
        var readSnippetError = snippetGroups.Errors.Single();
        ObjectApprover.VerifyWithJson(readSnippetError);
    }

    [Test]
    public void SortByPackage()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "key",
                version: new VersionRange(new NuGetVersion (1, 0, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package3"),
            new ReadSnippet(
                key: "key",
                version: new VersionRange(new NuGetVersion (1, 0, 0)),
                value: "3",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "key",
                language: string.Empty,
                version: new VersionRange(new NuGetVersion (1, 0, 0)),
                value: "4",
                startLine: 1,
                endLine: 1,
                path: string.Empty,
                package: "package2"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets, (key, packages) => packages.OrderBy(x=>x.Package).ToList()).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }

    [Test]
    public void Simple()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                version: new VersionRange(new NuGetVersion (1, 2, 0)),
                value: "1",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey1",
                version: new VersionRange(new NuGetVersion (1, 4, 0)),
                value: "2",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                version: new VersionRange(new NuGetVersion (1, 3, 0)),
                value: "3",
                startLine: 1,
                endLine: 1,
                path: null,
                language: "cs",
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                language: "cs",
                version: new VersionRange(new NuGetVersion (1, 4, 0)),
                value: "4",
                startLine: 1,
                endLine: 1,
                path: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                language: "cs",
                version: new VersionRange(new NuGetVersion (1, 5, 0)),
                value: "4",
                startLine: 1,
                endLine: 1,
                path: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                language: "cs",
                version: new VersionRange(new NuGetVersion (1, 6, 0)),
                value: "5",
                startLine: 1,
                endLine: 1,
                path: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey2",
                language: "cs",
                version: new VersionRange(new NuGetVersion(1, 7, 0)),
                value: "5",
                startLine: 1,
                endLine: 1,
                path: string.Empty,
                package: "package1"),
        };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }

    [Test]
    public void Merging()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                version: VersionRange.Parse("1.2.0"),
                value: "code",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey1",
                version: VersionRange.Parse("1.4.0"),
                value: "code",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
            new ReadSnippet(
                key: "foundkey1",
                version: VersionRange.Parse("1.3.0"),
                value: "code",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1"),
        };
        var snippetGroups = SnippetGrouper.ProcessKeyGroup(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }

    [Test]
    public void Single()
    {
        var snippets = new List<ReadSnippet>
        {
            new ReadSnippet(
                key: "foundkey1",
                version: VersionRange.Parse("1.2.0"),
                value: "code",
                startLine: 1,
                endLine: 1,
                path: null,
                language: string.Empty,
                package: "package1")
        };
        var snippetGroups = SnippetGrouper.ProcessKeyGroup(snippets).ToList();
        ObjectApprover.VerifyWithJson(snippetGroups);
    }
}