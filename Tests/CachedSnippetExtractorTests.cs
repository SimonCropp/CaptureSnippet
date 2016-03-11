using System.Diagnostics;
using System.Linq;
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
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup 
        var snippetExtractor = new CachedSnippetExtractor(
            versionExtractor: (x,y) => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageExtractor: (x, y) => null);
        snippetExtractor.FromDirectory(directory).GetAwaiter().GetResult();

        var cachedSnippetExtractor = new CachedSnippetExtractor(
            versionExtractor: (x, y) => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageExtractor: (x, y) => null);
        var firstRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory).GetAwaiter().GetResult();
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory).GetAwaiter().GetResult();
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Trace.WriteLine(firstRun.ElapsedMilliseconds);
        Trace.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public void EnsureErrorsAreReturned()
    {
        var directory = @"badsnippets".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(
            versionExtractor: (x, y) => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageExtractor: (x, y) => null);
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory).Result;
        Assert.AreEqual(1,readSnippets.GroupingErrors.Count());
    }

}