using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleSnippetMarkdownHandling
    {
        public static void AppendGroup(string key, IReadOnlyList<Snippet> group, TextWriter writer)
        {
            Guard.AgainstNull(group, nameof(group));
            Guard.AgainstNull(writer, nameof(writer));

            writer.WriteLine($"### Key: '{key}'");
            foreach (var snippet in group)
            {
                WriteSnippet(writer, snippet);
            }
        }

        static void WriteSnippet(TextWriter writer, Snippet snippet)
        {
            if (snippet.IsShared)
            {
                writer.WriteLine("#### Shared");
                return;
            }
            writer.WriteLine($"#### Package: {snippet.Package}. Version: {snippet.Version.ToFriendlyString()}");
            var format = $@"```{snippet.Language}
{snippet.Value}
```";
            writer.WriteLine(format);
        }
    }
}