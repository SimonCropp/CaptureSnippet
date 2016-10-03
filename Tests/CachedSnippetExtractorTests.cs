using System.Diagnostics;
using ApprovalTests.Reporters;
using CaptureSnippets;
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
        var extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        extractor.FromDirectory(directory);

        extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        extractor.FromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        extractor.FromDirectory(directory);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Trace.WriteLine(firstRun.ElapsedMilliseconds);
        Trace.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public void EnsureErrorsAreReturned()
    {
        var directory = "badsnippets".ToCurrentDirectory();
        var extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var read = extractor.FromDirectory(directory);
        Assert.AreEqual(1, read.Components.SnippetsInError.Count);
    }

}