using System.Diagnostics;
using System.IO;
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
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        cachedSnippetExtractor.FromDirectory(directory);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedMilliseconds, Is.LessThan(firstRun.ElapsedMilliseconds));
        Debug.WriteLine(firstRun.ElapsedMilliseconds);
        Debug.WriteLine(secondRun.ElapsedMilliseconds);
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