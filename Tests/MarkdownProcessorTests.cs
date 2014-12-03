using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class MarkdownProcessorTests
{

    [Test]
    public void Simple()
    {
        var availableSnippets = new List<SnippetGroup>
                                {
                                    new SnippetGroup
                                    {
                                        Key = "versionedSnippet1",
                                        Versions = new List<VersionGroup>
                                                   {
                                                       CreateVersionGroup(5),
                                                       CreateVersionGroup(4),
                                                   }
                                    },
                                    new SnippetGroup
                                    {
                                        Key = "versionedSnippet2",
                                        Versions = new List<VersionGroup>
                                                   {
                                                       CreateVersionGroup(3),
                                                       CreateVersionGroup(2),
                                                   }
                                    },
                                    new SnippetGroup
                                    {
                                        Key = "nonVersionedSnippet1",
                                        Versions = new List<VersionGroup>
                                                   {
                                                       CreateVersionGroup(5),
                                                   }
                                    },
                                    new SnippetGroup
                                    {
                                        Key = "nonVersionedSnippet2",
                                        Versions = new List<VersionGroup>
                                                   {
                                                       CreateVersionGroup(5),
                                                   }
                                    },
                                };
        var markdownContent = @"
<!-- import versionedSnippet1 -->

some text

<!-- import versionedSnippet2 -->

some other text

<!-- import nonVersionedSnippet1 -->

even more text

<!-- import nonVersionedSnippet2 -->

";
        var processor = new MarkdownProcessor();
        ObjectApprover.VerifyWithJson(processor.ApplyToText(availableSnippets, markdownContent), s => s.Replace("\\r\\n", "\r\n"));
    }

    static VersionGroup CreateVersionGroup(int version)
    {
        return new VersionGroup
        {
            Version = new Version(version, 0),
            Snippets = new List<Snippet>
                              {
                                  new Snippet
                                  {
                                      Language = "cs", 
                                      Value = "Snippet_v" + version
                                  }
                              }
        };
    }
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
        ObjectApprover.VerifyWithJson(result, s => s.Replace("\\r\\n", "\r\n"));
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
        ObjectApprover.VerifyWithJson(result, s => s.Replace("\\r\\n", "\r\n"));
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
        ObjectApprover.VerifyWithJson(result, s => s.Replace("\\r\\n", "\r\n"));
    }
}