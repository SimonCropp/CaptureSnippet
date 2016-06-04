using System.Diagnostics;
using System.Threading.Tasks;
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
    public async Task SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = "scenarios".ToCurrentDirectory();
        //warmup
        var result = PathData.With(VersionRange.All, Package.Undefined);
        var extractor = new CachedSnippetExtractor(
            extractData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        await extractor.FromDirectory(directory);

        extractor = new CachedSnippetExtractor(
            extractData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        await extractor.FromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        await extractor.FromDirectory(directory);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Trace.WriteLine(firstRun.ElapsedMilliseconds);
        Trace.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public async Task EnsureErrorsAreReturned()
    {
        var result = PathData.With(VersionRange.All, Package.Undefined);
        var directory = "badsnippets".ToCurrentDirectory();
        var extractor = new CachedSnippetExtractor(
            extractData: y => result,
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var read = await extractor.FromDirectory(directory);
        Assert.AreEqual(1, read.GroupingErrors.Count);
    }

}