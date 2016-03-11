using ApprovalTests.Reporters;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
[UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
public class MarkdownProcessor_TryExtractKeyFromLegacyTests
{

    [Test]
    public void MissingSpaces()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLineLegacy("<!--import mycodesnippet-->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }

    [Test]
    public void MissingSpacesWithNonChars()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLineLegacy("<!--import mycodesnippet_-->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }

    [Test]
    public void WithDashes()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLineLegacy("<!-- import my-code-snippet -->", out key);
        Assert.AreEqual("my-code-snippet", key);
    }

    [Test]
    public void Simple()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLineLegacy("<!-- import mycodesnippet -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }

    [Test]
    public void ExtraSpace()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLineLegacy("<!--  import  mycodesnippet  -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }
}