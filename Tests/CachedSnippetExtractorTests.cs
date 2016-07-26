using System.Diagnostics;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class CachedSnippetExtractorTests
{
    [Test]
    [Explicit]
    public void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = "scenarios".ToCurrentDirectory();
        //warmup
        var result = PathData.With(VersionRange.All, Package.Undefined, Component.Undefined);
        var extractor = new CachedSnippetExtractor(
            extractFileNameData: y => result,
            extractDirectoryPathData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        extractor.FromDirectory(directory, VersionRange.All, Package.Undefined, Component.Undefined);

        extractor = new CachedSnippetExtractor(
            extractFileNameData: y => result,
            extractDirectoryPathData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        extractor.FromDirectory(directory, VersionRange.All, Package.Undefined, Component.Undefined);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        extractor.FromDirectory(directory, VersionRange.All, Package.Undefined, Component.Undefined);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Trace.WriteLine(firstRun.ElapsedMilliseconds);
        Trace.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public void EnsureErrorsAreReturned()
    {
        var result = PathData.With(VersionRange.All, Package.Undefined, Component.Undefined);
        var directory = "badsnippets".ToCurrentDirectory();
        var extractor = new CachedSnippetExtractor(
            extractDirectoryPathData: y => result,
            extractFileNameData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var read = extractor.FromDirectory(directory, VersionRange.All, Package.Undefined, Component.Undefined);
        Assert.AreEqual(1, read.GroupingErrors.Count);
    }

}