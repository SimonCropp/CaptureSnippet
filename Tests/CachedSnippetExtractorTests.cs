using System.Diagnostics;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class CachedSnippetExtractorTests
{
    [Test]
    public void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup 
        new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs")).FromDirectory(directory);

        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Debug.WriteLine(firstRun.ElapsedTicks);
        Debug.WriteLine(secondRun.ElapsedTicks);
    }

    [Test]
    public void AssertOutput()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory);
        ObjectApprover.VerifyWithJson(readSnippets,s => s.Replace(directory.Replace("\\","\\\\"), ""));
    }
}