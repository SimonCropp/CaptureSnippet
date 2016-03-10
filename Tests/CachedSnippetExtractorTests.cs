using System.Diagnostics;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;
using NUnit.Framework;

[TestFixture]
public class CachedSnippetExtractorTests
{
    [Test]
    [Explicit]
    public void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup 
        var snippetExtractor = new CachedSnippetExtractor(
            versionFromFilePathExtractor: s => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageFromFilePathExtractor:s => null);
        snippetExtractor.FromDirectory(directory).GetAwaiter().GetResult();

        var cachedSnippetExtractor = new CachedSnippetExtractor(
            versionFromFilePathExtractor: s => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageFromFilePathExtractor: s => null);
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
            versionFromFilePathExtractor: s => VersionRange.All, 
            includeDirectory: s => true, 
            includeFile: s => s.EndsWith(".cs"),
            packageFromFilePathExtractor: s => null);
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory).Result;
        Assert.AreEqual(1,readSnippets.GroupingErrors.Count());
    }

}