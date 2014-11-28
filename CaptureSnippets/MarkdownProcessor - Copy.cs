using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    public class MarkdownProcessor
    {
        public ProcessResult Apply(List<ReadSnippet> snippets, string inputFile)
        {
            using (var reader = IndexReader.FromFile(inputFile))
            {
                return Apply(snippets, reader);
            }
        }

        public ProcessResult ApplyToText(List<ReadSnippet> availableSnippets, string markdownContent)
        {
            using (var reader = IndexReader.FromString(markdownContent))
            {
                return Apply(availableSnippets, reader);
            }
        }

        ProcessResult Apply(List<ReadSnippet> availableSnippets, IndexReader reader)
        {
            var stringBuilder = new StringBuilder();
            var lookup = new Dictionary<string, ReadSnippet>(StringComparer.OrdinalIgnoreCase);
            foreach (var snippet in availableSnippets)
            {
                lookup[snippet.Key] = snippet;
            }
            var result = new ProcessResult();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                stringBuilder.AppendLine(line);

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }
                ReadSnippet codeSnippet;
                if (!lookup.TryGetValue(key, out codeSnippet))
                {
                    var missingSnippet = new MissingSnippet
                    {
                        Key = key,
                        Line = reader.Index
                    };
                    result.MissingSnippet.Add(missingSnippet);
                    stringBuilder.AppendLine(string.Format("** Could not find key '{0}' **", key));
                    continue;
                }

                AppendSnippet(codeSnippet, stringBuilder);
                result.UsedSnippets.Add(codeSnippet);
            }
            result.Text = stringBuilder.ToString().TrimTrailingNewLine();
            return result;
        }

        static void AppendSnippet(ReadSnippet codeSnippet, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("```" + codeSnippet.Language);
            stringBuilder.AppendLine(codeSnippet.Value);
            stringBuilder.AppendLine("```");
        }
    }
}