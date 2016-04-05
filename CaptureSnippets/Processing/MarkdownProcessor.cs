using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaptureSnippets
{

    /// <summary>
    /// Merges <see cref="SnippetGroup"/>s with an input file/text.
    /// </summary>
    public static class MarkdownProcessor
    {

        /// <summary>
        /// Apply <paramref name="snippets"/> to <paramref name="textReader"/>.
        /// </summary>
        public static async Task<ProcessResult> Apply(IEnumerable<SnippetGroup> snippets, TextReader textReader, TextWriter writer, AppendGroupToMarkdown appendGroupToMarkdown)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(textReader, "textReader");
            Guard.AgainstNull(writer, "writer");
            using (var reader = new IndexReader(textReader))
            {
                return await Apply(snippets, writer, reader, appendGroupToMarkdown)
                    .ConfigureAwait(false);
            }
        }

        static async Task<ProcessResult> Apply(IEnumerable<SnippetGroup> availableSnippets, TextWriter writer, IndexReader reader, AppendGroupToMarkdown appendGroupToMarkdown)
        {
            var snippets = availableSnippets.ToList();
            var missingSnippets = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = await reader.ReadLine().ConfigureAwait(false)) != null)
            {
                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    await writer.WriteLineAsync(line)
                        .ConfigureAwait(false);
                    continue;
                }
                await writer.WriteLineAsync($"<!-- snippet: {key} -->")
                    .ConfigureAwait(false);

                var snippetGroup = snippets.FirstOrDefault(x => x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet(key: key, line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    var message = $"** Could not find key '{key}' **";
                    await writer.WriteLineAsync(message)
                        .ConfigureAwait(false);
                    continue;
                }

                await appendGroupToMarkdown(snippetGroup, writer)
                    .ConfigureAwait(false);
                if (usedSnippets.Any(x => x.Key == snippetGroup.Key))
                {
                    throw new Exception($"Duplicate use of the same snippet key '{snippetGroup.Key}'.");
                }
                usedSnippets.Add(snippetGroup);
            }
            return new ProcessResult(missingSnippets: missingSnippets, usedSnippets: usedSnippets);
        }

    }
}