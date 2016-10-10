using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class DirectorySnippetExtractorTests
{
    [Test]
    public void Case()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor/Case");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: _ => true,
            fileFilter: _ => true,
            packageOrder: _ => new List<string>()
        );
        var components = extractor.ReadComponents(directory);
        AssertCaseInsensitive(components.Lookup);
        var component = components.GetComponent("ComponentY");
        AssertCaseInsensitive(component.Lookup);
        var package = component.Packages.Single();
        AssertCaseInsensitive(package.Lookup);
        AssertCaseInsensitive(package.Versions.First().Lookup);
        ObjectApprover.VerifyWithJson(components, Scrubber.Scrub);
    }

    static void AssertCaseInsensitive(IReadOnlyDictionary<string, IReadOnlyList<Snippet>> dictionary)
    {
        Assert.IsTrue(dictionary.ContainsKey("GlobalSharedSnippet"));
        Assert.IsTrue(dictionary.ContainsKey("globalsharedSnippet"));
        Assert.IsTrue(dictionary.ContainsKey("ComponentSharedSnippet"));
        Assert.IsTrue(dictionary.ContainsKey("componentSharedSnippet"));
        Assert.IsTrue(dictionary.ContainsKey("Snippet"));
        Assert.IsTrue(dictionary.ContainsKey("snippet"));
    }

    [Test]
    public void Nested()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor/Nested");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path => true,
            fileFilter: path => true,
            packageOrder: component => new List<string>()
        );
        var components = extractor.ReadComponents(directory);
        ObjectApprover.VerifyWithJson(components.AllSnippets, Scrubber.Scrub);
    }

    [Test]
    public void Simple()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor/Simple");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path => true,
            fileFilter: path => true,
            packageOrder: component => new List<string>()
        );
        var components = extractor.ReadComponents(directory);
        ObjectApprover.VerifyWithJson(components, Scrubber.Scrub);
    }

    [Test]
    public void Sorting()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor/Sorting");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path => true,
            fileFilter: path => true,
            packageOrder: PackageOrder);
        var components = extractor.ReadComponents(directory);
        var snippets = components
            .Components
            .SelectMany(_ => _.Packages)
            .SelectMany(_ => _.Snippets)
            .Select(_ => $"{_.Package} {_.Version.SimplePrint()} {_.IsCurrent}");
        ObjectApprover.VerifyWithJson(snippets, Scrubber.Scrub);
    }

    IEnumerable<string> PackageOrder(string component)
    {
        if (component == "componentA")
        {
            yield return "packageC";
            yield return "packageA";
            yield return "packageB";
        }
        if (component == "componentB")
        {
            yield return "packageE";
            yield return "packageD";
        }
        if (component == "componentC")
        {
            yield return "packageG";
            yield return "packageF";
        }
    }


    [Test]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<CapturedDirectory>();
        var files = new ConcurrentBag<CapturedFile>();
        var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,
            "DirectorySnippetExtractor/VerifyLambdasAreCalled");
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path =>
            {
                var capture = new CapturedDirectory
                {
                    Path = path
                };
                directories.Add(capture);
                return true;
            },
            fileFilter: path =>
            {
                var capture = new CapturedFile
                {
                    Path = path
                };
                files.Add(capture);
                return true;
            },
            packageOrder: component => new List<string>()
        );
        extractor.ReadComponents(targetDirectory);
        result.Files = files.OrderBy(file => file.Path).ToList();
        result.Directories = directories.OrderBy(file => file.Path).ToList();
        ObjectApprover.VerifyWithJson(result, Scrubber.Scrub);
    }


    public class TestResult
    {
        public List<CapturedDirectory> Directories;
        public List<CapturedFile> Files;
    }

    public class CapturedDirectory
    {
        public string Path;
    }

    public class CapturedFile
    {
        public string Path;
    }

}