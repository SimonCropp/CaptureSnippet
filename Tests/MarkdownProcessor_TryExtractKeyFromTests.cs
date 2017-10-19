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
        var exception = Assert.Throws<Exception>(() => SnippetKeyReader.TryExtractKeyFromLine("snippet:snippet", out var key));
        Assert.IsTrue(exception.Message == "Invalid syntax for the snippet 'snippet': There must be a space before the start of the key.");
    }


    [Test]
    public void WithDashes()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet: my-code-snippet", out var key);
        Assert.AreEqual("my-code-snippet", key);
    }

    [Test]
    public void Simple()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet: snippet", out var key);
        Assert.AreEqual("snippet", key);
    }

    [Test]
    public void ExtraSpace()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet:  snippet   ", out var key);
        Assert.AreEqual("snippet", key);
    }
}