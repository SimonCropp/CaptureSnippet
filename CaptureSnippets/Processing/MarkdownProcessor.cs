using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var includesWithAll = GetIncludesWithAll(this.includes).ToList();
            ProcessNestedIncludes(includesWithAll, this.includes);
            this.appendIncludeGroup = appendIncludeGroup;
            this.snippets = snippets.ToList();
            this.appendSnippetGroup = appendSnippetGroup;
        }

        void ProcessNestedIncludes(List<IncludeKeyAndSource> includesWithAll, List<IncludeGroup> tempIncludes)
        {
            foreach (var include in tempIncludes)
            {
                foreach (var package in include.Packages)
                {
                    foreach (var versionGroup in package)
                    {
                        versionGroup.Value = Process(includesWithAll, include.Key, versionGroup.Value);
                    }
                }
            }
        }

        string Process(List<IncludeKeyAndSource> includesWithAll, string keyBeingProcessed, string markdown)
        {
            var count = 0;
            var resultmarkdown = markdown;
            while (true)
            {
                count++;
                if (count == 10)
                {
                    throw new Exception("Recursive include detected");
                }
                if (!InnerProcess(includesWithAll, keyBeingProcessed, ref resultmarkdown))
                {
                    break;
                }
            }
            return resultmarkdown;
        }

        bool InnerProcess(List<IncludeKeyAndSource> includesWithAll, string keyBeingProcessed, ref string resultmarkdown)
        {
            var foundInclude = false;
            using (var reader = new StringReader(resultmarkdown))
            {
                var builder = new StringBuilder();
                var lineNumber = 0;
                while (true)
                {
                    lineNumber++;
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string key;
                    if (!IncludeKeyReader.TryExtractKeyFromLine(line, out key))
                    {
                        builder.AppendLine(line);
                        continue;
                    }
                    var keyAndSource = includesWithAll.SingleOrDefault(source => source.Key == key);
                    if (keyAndSource == null)
                    {
                        throw new Exception($"Failed to process include '{keyBeingProcessed}'. Could not find include '{key}'.");
                    }
                    foundInclude = true;
                    builder.AppendLine(keyAndSource.Source);
                }
                resultmarkdown = builder.ToString();
            }
            return foundInclude;
        }


        IEnumerable<IncludeKeyAndSource> GetIncludesWithAll(List<IncludeGroup> tempIncludes)
        {
            foreach (var include in tempIncludes)
            {
                if (include.Packages.Count > 1)
                {
                    continue;
                }
                foreach (var package in include.Packages)
                {
                    if (package.Versions.Count > 1)
                    {
                        continue;
                    }
                    yield return new IncludeKeyAndSource
                    {
                        Key = include.Key,
                        Source = package.Versions.Single().Value
                    };
                }
            }
        }

        class IncludeKeyAndSource
        {
            public string Key;
            public string Source;
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
                return await Apply(writer, reader);
            }
        }

        async Task<ProcessResult> Apply(TextWriter writer, IndexReader reader)
        {
            var missing = new List<MissingKey>();
            var usedSnippets = new List<SnippetGroup>();
            var usedIncludes = new List<IncludeGroup>();
            string line;
            while ((line = await reader.ReadLine()) != null)
            {
                if (await TryProcessSnippetLine(writer, reader, line, missing, usedSnippets))
                {
                    continue;
                }
                if (await TryProcessIncludeLine(writer, reader, line, missing, usedIncludes))
                {
                    continue;
                }
                await writer.WriteLineAsync(line);
            }
            return new ProcessResult(
                missing: missing,
                usedSnippets: usedSnippets,
                usedIncludes: usedIncludes);
        }

        async Task<bool> TryProcessIncludeLine(TextWriter writer, IndexReader reader, string line, List<MissingKey> missings, List<IncludeGroup> used)
        {
            string key;
            if (!IncludeKeyReader.TryExtractKeyFromLine(line, out key))
            {
                return false;
            }
            await writer.WriteLineAsync($"<!-- include: {key} -->");

            var group = includes.FirstOrDefault(x => x.Key == key);
            if (group == null)
            {
                var missing = new MissingKey(
                    key: key,
                    line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find include '{key}' **";
                await writer.WriteLineAsync(message);
                return true;
            }

            await appendIncludeGroup(group, writer);
            if (used.Any(x => x.Key == group.Key))
            {
                throw new Exception($"Duplicate use of the same snippet include '{group.Key}'.");
            }
            used.Add(group);
            return true;
        }

        async Task<bool> TryProcessSnippetLine(TextWriter writer, IndexReader reader, string line, List<MissingKey> missings, List<SnippetGroup> used)
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
                var missing = new MissingKey(
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