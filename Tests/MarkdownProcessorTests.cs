using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class MarkdownProcessor_TryExtractKeyFromTests
{

    [Test]
    public void MissingSpaces()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLine("<!--import mycodesnippet-->", out key);
        Assert.AreEqual("mycodesnippet",key);
    }

    [Test]
    public void MissingSpacesWithNonChars()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLine("<!--import mycodesnippet_-->", out key);
        Assert.AreEqual("mycodesnippet",key);
    }

    [Test]
    public void WithDashes()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLine("<!-- import my-code-snippet -->", out key);
        Assert.AreEqual("my-code-snippet",key);
    }

    [Test]
    public void Simple()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLine("<!-- import mycodesnippet -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }

    [Test]
    public void ExtraSpace()
    {
        string key;
        ImportKeyReader.TryExtractKeyFromLine("<!--  import  mycodesnippet  -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }
}