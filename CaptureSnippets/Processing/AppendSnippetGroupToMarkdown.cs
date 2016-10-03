using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    public delegate void AppendSnippetGroupToMarkdown(string key, IReadOnlyList<Snippet> snippetGroup, TextWriter writer);
}