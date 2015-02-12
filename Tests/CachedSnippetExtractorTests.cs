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
    public async void SecondReadShouldBeFasterThanFirstRead()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        //warmup 
        var snippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        await snippetExtractor.FromDirectory(directory).ConfigureAwait(false);

        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var firstRun = Stopwatch.StartNew();
        await cachedSnippetExtractor.FromDirectory(directory).ConfigureAwait(false);
        firstRun.Stop();
        var secondRun = Stopwatch.StartNew();
        await cachedSnippetExtractor.FromDirectory(directory).ConfigureAwait(false);
        secondRun.Stop();
        Assert.That(secondRun.ElapsedTicks, Is.LessThan(firstRun.ElapsedTicks));
        Debug.WriteLine(firstRun.ElapsedMilliseconds);
        Debug.WriteLine(secondRun.ElapsedMilliseconds);
    }

    [Test]
    public async void AssertOutput()
    {
        var directory = @"scenarios\01-UpdateSimpleFile".ToCurrentDirectory();
        var cachedSnippetExtractor = new CachedSnippetExtractor(s => null, s => true, s => s.EndsWith(".cs"));
        var readSnippets = await cachedSnippetExtractor.FromDirectory(directory).ConfigureAwait(false);
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