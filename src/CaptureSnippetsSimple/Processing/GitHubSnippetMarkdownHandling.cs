using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    /// <summary>
    /// GitHub markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public class GitHubSnippetMarkdownHandling
    {
        string repositoryRoot;

        public GitHubSnippetMarkdownHandling(string repositoryRoot)
        {
            Guard.AgainstNullAndEmpty(repositoryRoot, nameof(repositoryRoot));
            this.repositoryRoot = repositoryRoot.Replace(@"\", "/");
        }

        public void AppendGroup(string key, IReadOnlyList<Snippet> snippets, TextWriter writer)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(writer, nameof(writer));

            foreach (var snippet in snippets)
            {
                WriteSnippet(writer, snippet);
            }
        }

        void WriteSnippet(TextWriter writer, Snippet snippet)
        {
            var format = $@"```{snippet.Language}
{snippet.Value}
```";
            writer.WriteLine(format);
            var path = snippet.Path.Replace(@"\", "/").ReplaceCaseless(repositoryRoot,"");
            writer.WriteLine($"<sup>[snippet source]({path}#L{snippet.StartLine}-L{snippet.EndLine})</sup>");
        }
    }
}