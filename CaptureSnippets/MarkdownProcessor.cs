using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;

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
        public async Task<ProcessResult> Apply(IEnumerable<SnippetGroup> snippets, TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(textReader, "textReader");
            Guard.AgainstNull(writer, "writer");
            using (var reader = new IndexReader(textReader))
            {
                return await Apply(snippets, writer, reader)
                    .ConfigureAwait(false);
            }
        }
        
        async Task<ProcessResult> Apply(IEnumerable<SnippetGroup> availableSnippets, TextWriter writer, IndexReader reader)
        {
            var snippets = availableSnippets.ToList();
            var missingSnippets = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                await writer.WriteLineAsync(line)
                    .ConfigureAwait(false);

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }

                var snippetGroup = snippets.FirstOrDefault(x => x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet(key: key, line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    var message = string.Format("** Could not find key '{0}' **", key);
                    await writer.WriteLineAsync(message)
                        .ConfigureAwait(false);
                    continue;
                }

                await AppendGroup(snippetGroup, writer)
                    .ConfigureAwait(false);
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
        public async Task AppendGroup(SnippetGroup snippetGroup, TextWriter writer)
        {
            Guard.AgainstNull(snippetGroup, "snippetGroup");
            Guard.AgainstNull(writer, "writer");
            foreach (var versionGroup in snippetGroup)
            {
                await AppendVersionGroup(writer, versionGroup);
            }
        }

        async Task AppendVersionGroup(TextWriter writer, VersionGroup versionGroup)
        {
            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = string.Format("#### Version '{0}'", versionGroup.Version.ToFriendlyString());
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            foreach (var snippet in versionGroup)
            {
                await AppendSnippet(snippet, writer)
                    .ConfigureAwait(false);
            }
        }

        public async Task AppendSnippet(Snippet codeSnippet, TextWriter writer)
        {
            Guard.AgainstNull(codeSnippet, "codeSnippet");
            Guard.AgainstNull(writer, "writer");
            var format = string.Format(
@"```{0}
{1}
```", codeSnippet.Language, codeSnippet.Value);
            await writer.WriteLineAsync(format)
                .ConfigureAwait(false);
        }
    }
}