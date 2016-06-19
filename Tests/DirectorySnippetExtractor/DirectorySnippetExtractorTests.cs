using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class DirectorySnippetExtractorTests
{
    [Test]
    public void VerifyLambdasAreCalled()
    {
        var versionAndPaths = new ConcurrentBag<CapturedVersionAndPath>();
        var directories = new ConcurrentBag<CapturedDirectory>();
        var files = new ConcurrentBag<CapturedFile>();
        var translatePackages = new ConcurrentBag<CapturedTranslatePackage>();
        var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor");
        var data = PathData.With(VersionRange.All, "package", Component.Undefined);
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            extractPathData: path =>
            {
                var versionAndPath = new CapturedVersionAndPath
                {
                    Path = path
                };
                versionAndPaths.Add(versionAndPath);
                return data;
            },
            directoryFilter: path =>
            {
                directories.Add(new CapturedDirectory {Path = path});
                return true;
            },
            fileFilter: path =>
            {
                files.Add(new CapturedFile {Path = path});
                return true;
            },
            translatePackage: alias =>
            {
                translatePackages.Add(new CapturedTranslatePackage {Alias = alias});
                return alias;
            }
        );
        extractor.FromDirectory(targetDirectory, VersionRange.None, Package.Undefined, Component.Undefined)
            .GetAwaiter()
            .GetResult();
        result.Files = files.OrderBy(file => file.Path).ToList();
        result.Directories = directories.OrderBy(file => file.Path).ToList();
        result.VersionAndPaths = versionAndPaths.OrderBy(file => file.Path).ToList();
        result.TranslatePackages = translatePackages.OrderBy(file => file.Alias).ToList();
        ObjectApprover.VerifyWithJson(result, s => s.Replace(@"\\", @"\").Replace(targetDirectory, @"root\"));
    }

    public class CapturedTranslatePackage
    {
        public string Alias;
    }

    public class TestResult
    {
        public List<CapturedVersionAndPath> VersionAndPaths;
        public List<CapturedDirectory> Directories;
        public List<CapturedFile> Files;
        public List<CapturedTranslatePackage> TranslatePackages;
    }

    public class CapturedVersionAndPath
    {
        public string Path;
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