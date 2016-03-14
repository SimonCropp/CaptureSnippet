using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaptureSnippets
{
    

    /// <summary>
    /// Merges <see cref="ReadSnippet"/>s with an input file/text.
    /// </summary>
    public class SimpleMarkdownProcessor
    {
    

        /// <summary>
        /// Apply <paramref name="snippets"/> to <paramref name="textReader"/>.
        /// </summary>
        public async Task<SimpleProcessResult> Apply(IEnumerable<ReadSnippet> snippets, TextReader textReader, TextWriter writer)
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

        async Task<SimpleProcessResult> Apply(IEnumerable<ReadSnippet> availableSnippets, TextWriter writer, IndexReader reader)
        {
            var snippets = availableSnippets.ToList();
            var missingSnippets = new List<MissingSnippet>();
            var usedSnippets = new List<ReadSnippet>();
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

                var snippet = snippets.FirstOrDefault(x => x.Key == key);
                if (snippet == null)
                {
                    var missingSnippet = new MissingSnippet(key: key, line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    var message = $"** Could not find key '{key}' **";
                    await writer.WriteLineAsync(message)
                        .ConfigureAwait(false);
                    continue;
                }

                await AppendGroup(snippet, writer)
                    .ConfigureAwait(false);
                if (usedSnippets.Any(x => x.Key == snippet.Key))
                {
                    throw new Exception($"Duplicate use of the same snippet key '{snippet.Key}'.");
                }
                usedSnippets.Add(snippet);
            }
            return new SimpleProcessResult(missingSnippets: missingSnippets, usedSnippets: usedSnippets);
        }

        /// <summary>
        /// Method that can be override to control how an individual <see cref="ReadSnippet"/> is rendered.
        /// </summary>
        public Task AppendGroup(ReadSnippet snippet, TextWriter writer)
        {
            Guard.AgainstNull(snippet, "snippet");
            Guard.AgainstNull(writer, "writer");
            
            var format = $@"```{snippet.Language}
{snippet.Value}
```";
            return writer.WriteLineAsync(format);
        }

    }
}