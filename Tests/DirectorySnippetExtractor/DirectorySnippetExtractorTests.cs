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
        var includeDirectories = new ConcurrentBag<CapturedIncludeDirectory>();
        var includeFiles = new ConcurrentBag<CapturedIncludeFile>();
        var translatePackages = new ConcurrentBag<CapturedTranslatePackage>();
        var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor");
        var snippetMetaData = VersionAndPackage.With(VersionRange.All, "package");
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            extractVersionAndPackageFromPath: path =>
            {
                var versionAndPath = new CapturedVersionAndPath
                {
                    Path = path
                };
                versionAndPaths.Add(versionAndPath);
                return snippetMetaData;
            },
            includeDirectory: path =>
            {
                includeDirectories.Add(new CapturedIncludeDirectory {Path = path});
                return true;
            },
            includeFile: path =>
            {
                includeFiles.Add(new CapturedIncludeFile {Path = path});
                return true;
            },
            translatePackage: alias =>
            {
                translatePackages.Add(new CapturedTranslatePackage {Alias = alias});
                return alias;
            }
            );
        extractor.FromDirectory(targetDirectory)
            .GetAwaiter()
            .GetResult();
        result.IncludeFiles = includeFiles.OrderBy(file => file.Path).ToList();
        result.IncludeDirectories = includeDirectories.OrderBy(file => file.Path).ToList();
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
        public List<CapturedIncludeDirectory> IncludeDirectories;
        public List<CapturedIncludeFile> IncludeFiles;
        public List<CapturedTranslatePackage> TranslatePackages;
    }

    public class CapturedVersionAndPath
    {
        public string Path;
    }

    public class CapturedIncludeDirectory
    {
        public string Path;
    }

    public class CapturedIncludeFile
    {
        public string Path;
    }

}