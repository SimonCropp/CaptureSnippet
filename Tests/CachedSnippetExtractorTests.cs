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
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup
        var result = VersionAndPackage.With(VersionRange.All, Package.None);
        var snippetExtractor = new CachedSnippetExtractor(
            extractVersionAndPackageFromPath: y => result,
            includeDirectory: s => true,
            includeFile: s => s.EndsWith(".cs"));
        snippetExtractor.FromDirectory(directory).GetAwaiter().GetResult();

        var cachedSnippetExtractor = new CachedSnippetExtractor(
            extractVersionAndPackageFromPath: y => result,
            includeDirectory: s => true,
            includeFile: s => s.EndsWith(".cs"));
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
        var result = VersionAndPackage.With(VersionRange.All, Package.None);
        var directory = @"badsnippets".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(
            extractVersionAndPackageFromPath: y => result,
            includeDirectory: s => true,
            includeFile: s => s.EndsWith(".cs"));
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory).Result;
        Assert.AreEqual(1,readSnippets.GroupingErrors.Count);
    }

}