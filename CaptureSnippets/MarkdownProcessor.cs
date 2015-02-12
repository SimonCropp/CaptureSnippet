using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        public ProcessResult Apply(List<SnippetGroup> snippets, TextReader textReader)
        {
            using (var reader = new IndexReader(textReader))
            {
                return Apply(snippets, reader);
            }
        }

        ProcessResult Apply(List<SnippetGroup> availableSnippets, IndexReader reader)
        {
            var stringBuilder = new StringBuilder();
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

                var snippetGroup = availableSnippets.FirstOrDefault(x=>x.Key == key);
                if (snippetGroup == null)
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

                AppendGroup(snippetGroup, stringBuilder);
                if (result.UsedSnippets.All(x => x.Key != snippetGroup.Key))
                {
                    result.UsedSnippets.Add(snippetGroup);
                }
            }
            result.Text = stringBuilder.ToString().TrimTrailingNewLine();
            return result;
        }

        /// <summary>
        /// Method that cna be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public void AppendGroup(SnippetGroup snippetGroup, StringBuilder stringBuilder)
        {
            foreach (var versionGroup in snippetGroup)
            {
                if (versionGroup.Version != null)
                {
                    stringBuilder.AppendLine("#### Version " + versionGroup.Version);
                }
                foreach (var snippet in versionGroup)
                {
                    AppendSnippet(snippet, stringBuilder);
                }
            }
        }

        public void AppendSnippet(Snippet codeSnippet, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("```" + codeSnippet.Language);
            stringBuilder.AppendLine(codeSnippet.Value);
            stringBuilder.AppendLine("```");
        }
    }
}