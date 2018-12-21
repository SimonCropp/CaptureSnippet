using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;
using ObjectApproval;
using Xunit;

public class DirectorySnippetExtractorTests : TestBase
{
    [Fact]
    public void Case()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Case");
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
        Assert.True(dictionary.ContainsKey("Snippet"));
        Assert.True(dictionary.ContainsKey("snippet"));
    }

    [Fact]
    public void Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Nested");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path => true,
            fileFilter: path => true,
            packageOrder: component => new List<string>()
        );
        var components = extractor.ReadComponents(directory);
        ObjectApprover.VerifyWithJson(components.AllSnippets, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Simple");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path => true,
            fileFilter: path => true,
            packageOrder: component => new List<string>()
        );
        var components = extractor.ReadComponents(directory);
        ObjectApprover.VerifyWithJson(components, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<CapturedDirectory>();
        var files = new ConcurrentBag<CapturedFile>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
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