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
        List<IncludeGroup> includes;
        AppendSnippetGroupToMarkdown appendSnippetGroup;
        AppendIncludeGroupToMarkdown appendIncludeGroup;

        public MarkdownProcessor(
            IEnumerable<SnippetGroup> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup,
            IEnumerable<IncludeGroup> includes,
            AppendIncludeGroupToMarkdown appendIncludeGroup)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(appendSnippetGroup, "appendSnippetGroup");
            Guard.AgainstNull(includes, "includes");
            Guard.AgainstNull(appendIncludeGroup, "appendIncludeGroup");
            this.includes = includes.ToList();
            this.appendIncludeGroup = appendIncludeGroup;
            this.snippets = snippets.ToList();
            this.appendSnippetGroup = appendSnippetGroup;
        }

        /// <summary>
        /// Apply to <paramref name="writer"/>.
        /// </summary>
        public async Task<ProcessResult> Apply(TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(textReader, "textReader");
            Guard.AgainstNull(writer, "writer");
            using (var reader = new IndexReader(textReader))
            {
                return await Apply(writer, reader)
                    .ConfigureAwait(false);
            }
        }

        async Task<ProcessResult> Apply(TextWriter writer, IndexReader reader)
        {
            var missing = new List<MissingSnippetOrInclude>();
            var usedSnippets = new List<SnippetGroup>();
            var usedIncludes = new List<IncludeGroup>();
            string line;
            while ((line = await reader.ReadLine().ConfigureAwait(false)) != null)
            {
                if (await TryProcessSnippetLine(writer, reader, line, missing, usedSnippets)
                    .ConfigureAwait(false))
                {
                    continue;
                }
                if (await TryProcessIncludeLine(writer, reader, line, missing, usedIncludes)
                    .ConfigureAwait(false))
                {
                    continue;
                }
                await writer.WriteLineAsync(line)
                    .ConfigureAwait(false);
            }
            return new ProcessResult(
                missing: missing,
                usedSnippets: usedSnippets,
                usedIncludes: usedIncludes);
        }

        async Task<bool> TryProcessIncludeLine(TextWriter writer, IndexReader reader, string line, List<MissingSnippetOrInclude> missings, List<IncludeGroup> used)
        {
            string key;
            if (!IncludeKeyReader.TryExtractKeyFromLine(line, out key))
            {
                return false;
            }
            await writer.WriteLineAsync($"<!-- include: {key} -->")
                .ConfigureAwait(false);

            var group = includes.FirstOrDefault(x => x.Key == key);
            if (group == null)
            {
                var missing= new MissingSnippetOrInclude(key: key, line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find key '{key}' **";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
                return true;
            }

            await appendIncludeGroup(group, writer)
                .ConfigureAwait(false);
            if (used.Any(x => x.Key == group.Key))
            {
                throw new Exception($"Duplicate use of the same snippet key '{group.Key}'.");
            }
            used.Add(group);
            return true;
        }

        async Task<bool> TryProcessSnippetLine(TextWriter writer, IndexReader reader, string line, List<MissingSnippetOrInclude> missings, List<SnippetGroup> used)
        {
            string key;
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, out key))
            {
                return false;
            }
            await writer.WriteLineAsync($"<!-- snippet: {key} -->")
                .ConfigureAwait(false);

            var group = snippets.FirstOrDefault(x => x.Key == key);
            if (group == null)
            {
                var missing = new MissingSnippetOrInclude(key: key, line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find key '{key}' **";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
                return true;
            }

            await appendSnippetGroup(group, writer)
                .ConfigureAwait(false);
            if (used.Any(x => x.Key == group.Key))
            {
                throw new Exception($"Duplicate use of the same snippet key '{group.Key}'.");
            }
            used.Add(group);
            return true;
        }
    }
}