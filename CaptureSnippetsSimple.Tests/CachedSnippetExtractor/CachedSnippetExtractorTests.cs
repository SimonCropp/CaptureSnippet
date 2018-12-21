using System.Diagnostics;
using System.IO;
using CaptureSnippets;
using ObjectApproval;
using Xunit;

public class CachedSnippetExtractorTests : TestBase
{
    [Fact]
    public void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "CachedSnippetExtractor/Simple");
        //warmup
        var extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        extractor.ComponentsFromDirectory(directory);

        extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        extractor.ComponentsFromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        extractor.ComponentsFromDirectory(directory);
        secondRun.Stop();
        Assert.True(secondRun.ElapsedTicks < firstRun.ElapsedTicks);
        Trace.WriteLine(firstRun.ElapsedMilliseconds);
        Trace.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Fact]
    public void EnsureErrorsAreReturned()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "CachedSnippetExtractor/Bad");
        var extractor = new CachedSnippetExtractor(
            directoryFilter: s => true,
            fileFilter: s => true);
        var read = extractor.ComponentsFromDirectory(directory);
        ObjectApprover.VerifyWithJson(read.Components.SnippetsInError, Scrubber.Scrub);
    }
}