using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Snippets.Count}")]
    public class ReadSnippets
    {
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> Snippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;
        public readonly IReadOnlyList<Snippet> SnippetsInError;

        public ReadSnippets(string directory, IReadOnlyList<Snippet> snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Directory = directory;
            Snippets = snippets;
            SnippetsInError = Snippets.Where(_ => _.IsInError).Distinct().ToList();
            Lookup = Snippets.ToDictionary();
        }

    }
}