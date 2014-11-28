using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class DocumentFileProcessorTests
{
    [Test]
    public void MissingKey()
    {
        var snippets = new List<ReadSnippet>
        {
                new ReadSnippet
                    {
                        Key = "FoundKey1"
                    },
                new ReadSnippet
                    {
                        Key = "FoundKey2"
                    },
            };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        var result = new MarkdownProcessor().ApplyToText(snippetGroups, "<!-- import MissingKey -->");
            ObjectApprover.VerifyWithJson(result);
    }

    [Test]
    public void MissingMultipleKeys()
    {
        var snippets = new List<ReadSnippet>
            {
                new ReadSnippet
                    {
                        Key = "FoundKey1"
                    },
                new ReadSnippet
                    {
                        Key = "FoundKey2"
                    },
            };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        var result = new MarkdownProcessor().ApplyToText(snippetGroups, "<!-- import MissingKey1 -->\r\n\r\n<!-- import MissingKey2 -->");
        ObjectApprover.VerifyWithJson(result);
    }


    [Test]
    public void LotsOfText()
    {
        var snippets = new List<ReadSnippet>
            {
                new ReadSnippet
                    {
                        Key = "FoundKey1",
                        Value = "Value1"
                    },
                new ReadSnippet
                    {
                        Key = "FoundKey2",
                        Value = "Value2"
                    },
                new ReadSnippet
                    {
                        Key = "FoundKey3",
                        Value = "Value3"
                    },
                new ReadSnippet
                    {
                        Key = "FoundKey4",
                        Value = "Value4"
                    },
            };
        var snippetGroups = SnippetGrouper.Group(snippets).ToList();
        var result = new MarkdownProcessor().ApplyToText(snippetGroups, @"
<!-- import FoundKey2 -->\r\b\n<!-- import FoundKey1 -->
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn<!-- import FoundKey1 -->
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg<!-- import FoundKey1 -->
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg<!-- import FoundKey1 -->
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg<!-- import FoundKey1 -->
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg<!-- import FoundKey1 -->
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg<!-- import FoundKey1 -->
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn<!-- import FoundKey1 --><!-- import FoundKey1 -->
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmgfkgjnfdjkgn
dflkgmxdklfmgkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmdfgkjndfkjgngkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
kdjrngkjfncgdflkgmxdklfmgkdflxmg<!-- import FoundKey1 -->
kdjrngkjfncgdflkgmxdklfmgkdflxmg
dflkgmxdklfmgkdflxmg
lkmdflkgmxdklfmgkdflxmg
"); 
        ObjectApprover.VerifyWithJson(result);
    }
}