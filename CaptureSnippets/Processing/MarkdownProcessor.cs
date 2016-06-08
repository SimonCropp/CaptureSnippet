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
    public class MarkdownProcessor
    {
        List<SnippetGroup> snippets;
        AppendSnippetGroupToMarkdown appendSnippetGroup;

        public MarkdownProcessor(
            IEnumerable<SnippetGroup> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            this.snippets = snippets.ToList();
            this.appendSnippetGroup = appendSnippetGroup;
        }

        /// <summary>
        /// Apply to <paramref name="writer"/>.
        /// </summary>
        public async Task<ProcessResult> Apply(TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(writer, nameof(writer));
            using (var reader = new IndexReader(textReader))
            {
                return await Apply(writer, reader);
            }
        }

        async Task<ProcessResult> Apply(TextWriter writer, IndexReader reader)
        {
            var missing = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = await reader.ReadLine()) != null)
            {
                if (await TryProcessSnippetLine(writer, reader, line, missing, usedSnippets))
                {
                    continue;
                }
                await writer.WriteLineAsync(line);
            }
            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets);
        }

        async Task<bool> TryProcessSnippetLine(TextWriter writer, IndexReader reader, string line, List<MissingSnippet> missings, List<SnippetGroup> used)
        {
            string key;
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, out key))
            {
                return false;
            }
            await writer.WriteLineAsync($"<!-- snippet: {key} -->");

            var group = snippets.FirstOrDefault(x => x.Key == key);
            if (group == null)
            {
                var missing = new MissingSnippet(
                    key: key,
                    line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find snippet '{key}' **";
                await writer.WriteLineAsync(message);
                return true;
            }

            await appendSnippetGroup(group, writer);
            if (used.Any(x => x.Key == group.Key))
            {
                throw new Exception($"Duplicate use of the same snippet '{group.Key}'.");
            }
            used.Add(group);
            return true;
        }
    }
}