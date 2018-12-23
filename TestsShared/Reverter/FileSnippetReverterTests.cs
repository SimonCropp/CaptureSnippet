using ApprovalTests;
using CaptureSnippets;
using Xunit;

public class FileSnippetReverterTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var input = @"
aaa

<!-- snippet: snippet1 -->
### Key: 'snippet1'
####  Package: package1. Version: >= 5.0.0
```cs
Snippet_v[5.0.0, )
```
####  Package: package1. Version: >= 4.0.0
```cs
Snippet_v[4.0.0, )
```
<!-- endsnippet -->

bbb";
            Approvals.Verify(FileSnippetReverter.Revert(input));
    }
}