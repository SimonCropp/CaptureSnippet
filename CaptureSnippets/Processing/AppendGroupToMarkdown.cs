using System.IO;
using System.Threading.Tasks;

namespace CaptureSnippets
{
    public delegate Task AppendGroupToMarkdown(SnippetGroup snippetGroup, TextWriter writer);
}