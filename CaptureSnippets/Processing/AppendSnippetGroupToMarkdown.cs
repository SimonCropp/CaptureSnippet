using System.IO;
using System.Threading.Tasks;

namespace CaptureSnippets
{
    public delegate Task AppendSnippetGroupToMarkdown(SnippetGroup snippetGroup, TextWriter writer);
    public delegate Task AppendIncludeGroupToMarkdown(IncludeGroup includeGroup, TextWriter writer);
}