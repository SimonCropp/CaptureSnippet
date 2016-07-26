using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public ProcessResult Apply(TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(writer, nameof(writer));
            using (var reader = new IndexReader(textReader))
            {
                return Apply(writer, reader);
            }
        }

        ProcessResult Apply(TextWriter writer, IndexReader reader)
        {
            var missing = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (TryProcessSnippetLine(writer, reader, line, missing, usedSnippets))
                {
                    continue;
                }
                writer.WriteLine(line);
            }
            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets);
        }

        bool TryProcessSnippetLine(TextWriter writer, IndexReader reader, string line, List<MissingSnippet> missings, List<SnippetGroup> used)
        {
            string key;
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, out key))
            {
                return false;
            }
            writer.WriteLine($"<!-- snippet: {key} -->");

            var group = snippets.FirstOrDefault(x => x.Key == key);
            if (group == null)
            {
                var missing = new MissingSnippet(
                    key: key,
                    line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find snippet '{key}' **";
                writer.WriteLine(message);
                return true;
            }

            appendSnippetGroup(group, writer);
            if (used.Any(x => x.Key == group.Key))
            {
                throw new Exception($"Duplicate use of the same snippet '{group.Key}'.");
            }
            used.Add(group);
            return true;
        }
    }
}