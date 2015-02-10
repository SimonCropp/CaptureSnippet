using System;
using System.Diagnostics;
using System.Linq;
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
        Debug.WriteLine(firstRun.ElapsedMilliseconds);
        Debug.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public void AssertOutput()
    {
        var directory = @"scenarios\01-UpdateSimpleFile".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var readSnippets = cachedSnippetExtractor.FromDirectory(directory);
        ObjectApprover.VerifyWithJson(readSnippets,s => CleanOutput(s, directory));
    }

    static string CleanOutput(string s, string directory)
    {
        var replaced = s.ReplaceCaseless(directory.Replace("\\","\\\\"), "");
        var enumerable = replaced.Split(new[]
                                                {
                                                    "\n"
                                                }, StringSplitOptions.RemoveEmptyEntries)
                                                .Where(x => !x.Contains("\"Ticks\":"))
                                                .ToList();
        return String.Join("\n", enumerable);
    }
}