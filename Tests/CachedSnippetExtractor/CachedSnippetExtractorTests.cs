using System.Diagnostics;
using System.IO;
using ApprovalTests.Reporters;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class CachedSnippetExtractorTests
{
    [Test]
    public void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "CachedSnippetExtractor/Simple");
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
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "CachedSnippetExtractor/Bad");
        var extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => true);
        var read = extractor.FromDirectory(directory);
        ObjectApprover.VerifyWithJson(read.Components.SnippetsInError, Scrubber.Scrub);
    }

}