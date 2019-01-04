using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using CaptureSnippets;
using Xunit;

public class DirectorySnippetReverterTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), "Target");
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
        Directory.CreateDirectory(tempDirectory);
        try
        {
            var targetFile = Path.Combine(AssemblyLocation.CurrentDirectory, "Reverter/Simple/target.md");
            var tempFile = Path.Combine(tempDirectory, "target.md");
            File.Copy(targetFile, tempFile);
            var extractor = new DirectorySnippetReverter();
            extractor.Revert(tempDirectory);
            Approvals.Verify(File.ReadAllText(tempFile));
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    //[Fact]
    //public void VerifyLambdasAreCalled()
    //{
    //    var directories = new ConcurrentBag<CapturedDirectory>();
    //    var files = new ConcurrentBag<CapturedFile>();
    //    var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
    //        "DirectorySnippetExtractor/VerifyLambdasAreCalled");
    //    var result = new TestResult();
    //    var extractor = new DirectorySnippetReverter(
    //        directoryFilter: path =>
    //        {
    //            var capture = new CapturedDirectory
    //            {
    //                Path = path
    //            };
    //            directories.Add(capture);
    //            return true;
    //        },
    //        fileFilter: path =>
    //        {
    //            var capture = new CapturedFile
    //            {
    //                Path = path
    //            };
    //            files.Add(capture);
    //            return true;
    //        }
    //    );
    //    extractor.ReadSnippets(targetDirectory);
    //    result.Files = files.OrderBy(file => file.Path).ToList();
    //    result.Directories = directories.OrderBy(file => file.Path).ToList();
    //    ObjectApprover.VerifyWithJson(result, Scrubber.Scrub);
    //}

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