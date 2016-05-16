using System.Collections.Generic;
using System.IO;
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
        var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DirectorySnippetExtractor");
        var snippetMetaData = new SnippetMetaData(VersionRange.All, "package");
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            extractMetaDataFromPath: (rootDirectory, path, parent) =>
            {
                result.ExtractMetaDatas.Add(new CapturedExtractMetaData {RootDirectory = rootDirectory, Path = path, Parent = parent});
                return snippetMetaData;
            },
            includeDirectory: path =>
            {
                result.IncludeDirectories.Add(new CapturedIncludeDirectory {Path = path});
                return true;
            },
            includeFile: path =>
            {
                result.IncludeFiles.Add(new CapturedIncludeFile {Path = path});
                return true;
            },
            translatePackage: (path, alias) =>
            {
                result.TranslatePackages.Add(new CapturedTranslatePackage {Path = path, Alias = alias});
                return alias;
            },
            parseVersion: (version, path, metaDataForParentPath) =>
            {
                result.ParseVersions.Add(new CapturedParseVersion {Version = version, Path = path, MetaDataForParentPath = metaDataForParentPath});
                return VersionRange.All;
            });
        extractor.FromDirectory(targetDirectory)
            .GetAwaiter()
            .GetResult();
        ObjectApprover.VerifyWithJson(result, s => s.Replace(@"\\",@"\").Replace(targetDirectory, @"root\"));
    }

    public class TestResult
    {
        public List<CapturedExtractMetaData> ExtractMetaDatas = new List<CapturedExtractMetaData>();
        public List<CapturedIncludeDirectory> IncludeDirectories = new List<CapturedIncludeDirectory>();
        public List<CapturedIncludeFile> IncludeFiles = new List<CapturedIncludeFile>();
        public List<CapturedTranslatePackage> TranslatePackages = new List<CapturedTranslatePackage>();
        public List<CapturedParseVersion> ParseVersions = new List<CapturedParseVersion>();
    }

    public class CapturedExtractMetaData
    {
        public SnippetMetaData Parent;
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

    public class CapturedTranslatePackage
    {
        public string Path;
        public string Alias;
    }

    public class CapturedParseVersion
    {
        public string Version;
        public string Path;
        public SnippetMetaData MetaDataForParentPath;
    }
}