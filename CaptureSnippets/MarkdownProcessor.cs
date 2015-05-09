using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaptureSnippets
{

    /// <summary>
    /// Merges <see cref="SnippetGroup"/>s with an input file/text.
    /// </summary>
    public class MarkdownProcessor
    {

        /// <summary>
        /// Apply <paramref name="snippets"/> to <paramref name="textReader"/>.
        /// </summary>
        public async Task<ProcessResult> Apply(List<SnippetGroup> snippets, TextReader textReader, TextWriter writer)
        {
            using (var reader = new IndexReader(textReader))
            {
                return await Apply(snippets, writer, reader).ConfigureAwait(false);
            }
        }

        
        async Task<ProcessResult> Apply(List<SnippetGroup> availableSnippets, TextWriter writer, IndexReader reader)
        {
            var missingSnippets = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                await writer.WriteLineAsync(line).ConfigureAwait(false);

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }

                var snippetGroup = availableSnippets.FirstOrDefault(x => x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet(
                        key: key,
                        line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    await writer.WriteLineAsync(string.Format("** Could not find key '{0}' **", key)).ConfigureAwait(false);
                    continue;
                }

                await AppendGroup(snippetGroup, writer).ConfigureAwait(false);
                if (usedSnippets.All(x => x.Key != snippetGroup.Key))
                {
                    usedSnippets.Add(snippetGroup);
                }
            }
            return new ProcessResult(missingSnippets: missingSnippets, usedSnippets: usedSnippets);
        }

        /// <summary>
        /// Method that cna be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public async Task AppendGroup(SnippetGroup snippetGroup, TextWriter stringBuilder)
        {
            foreach (var versionGroup in snippetGroup)
            {
                if (versionGroup.Version != Version.ExplicitNull)
                {
                    await stringBuilder.WriteLineAsync("#### Version " + versionGroup.Version).ConfigureAwait(false);
                }
                foreach (var snippet in versionGroup)
                {
                    await AppendSnippet(snippet, stringBuilder).ConfigureAwait(false);
                }
            }
        }

        public async Task AppendSnippet(Snippet codeSnippet, TextWriter stringBuilder)
        {
            var format = string.Format(
@"```{0}
{1}
```", codeSnippet.Language, codeSnippet.Value);
            await stringBuilder.WriteLineAsync(format).ConfigureAwait(false);
        }
    }
}