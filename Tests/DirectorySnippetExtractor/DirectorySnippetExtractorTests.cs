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
        var extractMetaDatas = new ConcurrentBag<CapturedExtractMetaData>();
        var includeDirectories = new ConcurrentBag<CapturedIncludeDirectory>();
        var includeFiles = new ConcurrentBag<CapturedIncludeFile>();
        var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor");
        var snippetMetaData = SnippetMetaData.With(VersionRange.All, "package");
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            extractMetaDataFromPath: (rootDirectory, path) =>
            {
                var capturedExtractMetaData = new CapturedExtractMetaData
                {
                    RootDirectory = rootDirectory,
                    Path = path
                };
                extractMetaDatas.Add(capturedExtractMetaData);
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
            }
            );
        extractor.FromDirectory(targetDirectory)
            .GetAwaiter()
            .GetResult();
        result.IncludeFiles = includeFiles.OrderBy(file => file.Path).ToList();
        result.IncludeDirectories = includeDirectories.OrderBy(file => file.Path).ToList();
        result.ExtractMetaDatas = extractMetaDatas.OrderBy(file => file.Path).ToList();
        ObjectApprover.VerifyWithJson(result, s => s.Replace(@"\\", @"\").Replace(targetDirectory, @"root\"));
    }

    public class TestResult
    {
        public List<CapturedExtractMetaData> ExtractMetaDatas;
        public List<CapturedIncludeDirectory> IncludeDirectories;
        public List<CapturedIncludeFile> IncludeFiles;
    }

    public class CapturedExtractMetaData
    {
        public string Path;
        public string RootDirectory;
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