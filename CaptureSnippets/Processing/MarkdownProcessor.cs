using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public ProcessResult Apply(IEnumerable<SnippetGroup> snippets, TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(textReader, "textReader");
            Guard.AgainstNull(writer, "writer");
            using (var reader = new IndexReader(textReader))
            {
                return Apply(snippets, writer, reader);
            }
        }
        
        ProcessResult Apply(IEnumerable<SnippetGroup> availableSnippets, TextWriter writer, IndexReader reader)
        {
            var snippets = availableSnippets.ToList();
            var missingSnippets = new List<MissingSnippet>();
            var usedSnippets = new List<SnippetGroup>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLineAsync(line)
                    .ConfigureAwait(false);

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }

                var snippetGroup = snippets.FirstOrDefault(x => x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet(key: key, line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    var message = string.Format("** Could not find key '{0}' **", key);
                    writer.WriteLineAsync(message)
                        .ConfigureAwait(false);
                    continue;
                }

                AppendGroup(snippetGroup, writer);
                if (usedSnippets.All(x => x.Key != snippetGroup.Key))
                {
                    usedSnippets.Add(snippetGroup);
                }
            }
            return new ProcessResult(missingSnippets: missingSnippets, usedSnippets: usedSnippets);
        }

        /// <summary>
        /// Method that cna be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public void AppendGroup(SnippetGroup snippetGroup, TextWriter writer)
        {
            Guard.AgainstNull(snippetGroup, "snippetGroup");
            Guard.AgainstNull(writer, "writer");
            foreach (var versionGroup in snippetGroup)
            {
                AppendVersionGroup(writer, versionGroup,snippetGroup.Language);
            }
        }

        public void AppendVersionGroup(TextWriter writer, VersionGroup versionGroup, string language)
        {
            Guard.AgainstNull(versionGroup, "versionGroup");
            Guard.AgainstNull(writer, "writer");
            Guard.AgainstNullAndEmpty(language, "language");

            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = string.Format("#### Version '{0}'", versionGroup.Version.ToFriendlyString());
                writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            var format = string.Format(
@"```{0}
{1}
```", language, versionGroup.Value);
            writer.WriteLineAsync(format)
                .ConfigureAwait(false);
        }

    }
}