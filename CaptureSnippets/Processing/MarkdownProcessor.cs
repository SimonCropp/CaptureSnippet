using System;
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

                await AppendGroup(snippetGroup, writer)
                    .ConfigureAwait(false);
                if (usedSnippets.Any(x => x.Key == snippetGroup.Key))
                {
                    throw new Exception($"Duplicate use of the same snippet key '{snippetGroup.Key}'.");
                }
                usedSnippets.Add(snippetGroup);
            }
            return new ProcessResult(missingSnippets: missingSnippets, usedSnippets: usedSnippets);
        }

        /// <summary>
        /// Method that can be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public Task AppendGroup(SnippetGroup snippetGroup, TextWriter writer)
        {
            Guard.AgainstNull(snippetGroup, "snippetGroup");
            Guard.AgainstNull(writer, "writer");
            return Task.WhenAll(snippetGroup.Select(group => AppendVersionGroup(writer, group, snippetGroup.Language)));
        }

        public async Task AppendVersionGroup(TextWriter writer, VersionGroup versionGroup, string language)
        {
            Guard.AgainstNull(versionGroup, "versionGroup");
            Guard.AgainstNull(writer, "writer");
            Guard.AgainstNullAndEmpty(language, "language");

            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = $"#### Version '{versionGroup.Version.ToFriendlyString()}'";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            var format = $@"```{language}
{versionGroup.Value}
```";
            await writer.WriteLineAsync(format)
                .ConfigureAwait(false);
        }

    }
}