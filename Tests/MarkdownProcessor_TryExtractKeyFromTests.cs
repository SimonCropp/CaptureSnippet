using ApprovalTests.Reporters;
using CaptureSnippets;
using NUnit.Framework;
// ReSharper disable StringLiteralTypo

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class MarkdownProcessor_TryExtractKeyFromTests
{

    [Test]
    public void MissingSpaces()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet:mycodesnippet", out key);
        Assert.AreEqual("mycodesnippet", key);
    }


    [Test]
    public void WithDashes()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet:my-code-snippet", out key);
        Assert.AreEqual("my-code-snippet", key);
    }

    [Test]
    public void Simple()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet:mycodesnippet", out key);
        Assert.AreEqual("mycodesnippet", key);
    }

    [Test]
    public void ExtraSpace()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet:  mycodesnippet   ", out key);
        Assert.AreEqual("mycodesnippet", key);
    }
}