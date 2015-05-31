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
    public async void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup 
        var snippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        await snippetExtractor.FromDirectory(directory);

        var cachedSnippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        await cachedSnippetExtractor.FromDirectory(directory)
            .ConfigureAwait(false);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        await cachedSnippetExtractor.FromDirectory(directory)
            .ConfigureAwait(false);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Debug.WriteLine(firstRun.ElapsedMilliseconds);
        Debug.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public async void EnsureErrorsAreReturned()
    {
        var directory = @"badsnippets".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => VersionRange.All, s => true, s => s.EndsWith(".cs"));
        var readSnippets = await cachedSnippetExtractor.FromDirectory(directory);
        Assert.AreEqual(1,readSnippets.GroupingErrors.Count());
    }

}