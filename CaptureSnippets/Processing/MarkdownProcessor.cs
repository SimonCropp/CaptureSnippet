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

                string key;
                if (!ImportKeyReader.TryExtractKeyFromLine(line, out key))
                {
                    writer.WriteLine(line);
                    continue;
                }
                writer.WriteLine("<!-- snippet: {0} -->", key);

                var snippetGroup = snippets.FirstOrDefault(x => x.Key == key);
                if (snippetGroup == null)
                {
                    var missingSnippet = new MissingSnippet(key: key, line: reader.Index);
                    missingSnippets.Add(missingSnippet);
                    var message = $"** Could not find key '{key}' **";
                    writer.WriteLine(message);
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
                var message = $"#### Version '{versionGroup.Version.ToFriendlyString()}'";
                writer.WriteLine(message);
            }
            var format = $@"```{language}
{versionGroup.Value}
```";
            writer.WriteLine(format);
        }

    }
}