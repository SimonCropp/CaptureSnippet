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
        var snippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        snippetExtractor.FromDirectory(directory);

        var cachedSnippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Debug.WriteLine(firstRun.ElapsedMilliseconds);
        Debug.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public void EnsureErrorsAreReturned()
    {
        var directory = @"badsnippets".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory);
        Assert.AreEqual(1,readSnippets.GroupingErrors.Count());
    }

}