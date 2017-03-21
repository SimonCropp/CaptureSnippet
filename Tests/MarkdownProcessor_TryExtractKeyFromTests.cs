using System;
using ApprovalTests.Reporters;
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
        var exception = Assert.Throws<Exception>(() => SnippetKeyReader.TryExtractKeyFromLine("snippet:snippet", out key));
        Assert.IsTrue(exception.Message == "Invalid syntax for the snippet 'snippet': There must be a space before the start of the key.");
    }


    [Test]
    public void WithDashes()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet: my-code-snippet", out key);
        Assert.AreEqual("my-code-snippet", key);
    }

    [Test]
    public void Simple()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet: snippet", out key);
        Assert.AreEqual("snippet", key);
    }

    [Test]
    public void ExtraSpace()
    {
        string key;
        SnippetKeyReader.TryExtractKeyFromLine("snippet:  snippet   ", out key);
        Assert.AreEqual("snippet", key);
    }
}