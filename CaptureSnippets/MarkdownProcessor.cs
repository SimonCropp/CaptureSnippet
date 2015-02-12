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

        /// <summary>
        /// Apply <paramref name="snippets"/> to <paramref name="textReader"/>.
        /// </summary>
        public ProcessResult Apply(List<SnippetGroup> snippets, TextReader textReader, TextWriter writer)
        {
            using (var reader = new IndexReader(textReader))
            {
                return Apply(snippets, writer, reader);
            }
        }

        ProcessResult Apply(List<SnippetGroup> availableSnippets, TextWriter writer, IndexReader reader)
        {
            var result = new ProcessResult();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line);

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }

                var snippetGroup = availableSnippets.FirstOrDefault(x=>x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet
                    {
                        Key = key,
                        Line = reader.Index
                    };
                    result.MissingSnippet.Add(missingSnippet);
                    writer.WriteLine("** Could not find key '{0}' **", key);
                    continue;
                }

                AppendGroup(snippetGroup, writer);
                if (result.UsedSnippets.All(x => x.Key != snippetGroup.Key))
                {
                    result.UsedSnippets.Add(snippetGroup);
                }
            }
            return result;
        }

        /// <summary>
        /// Method that cna be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public void AppendGroup(SnippetGroup snippetGroup, TextWriter stringBuilder)
        {
            foreach (var versionGroup in snippetGroup)
            {
                if (versionGroup.Version != null)
                {
                    stringBuilder.WriteLine("#### Version " + versionGroup.Version);
                }
                foreach (var snippet in versionGroup)
                {
                    AppendSnippet(snippet, stringBuilder);
                }
            }
        }

        public void AppendSnippet(Snippet codeSnippet, TextWriter stringBuilder)
        {
            stringBuilder.WriteLine("```" + codeSnippet.Language);
            stringBuilder.WriteLine(codeSnippet.Value);
            stringBuilder.WriteLine("```");
        }
    }
}