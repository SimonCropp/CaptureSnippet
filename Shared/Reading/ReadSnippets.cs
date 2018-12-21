using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Snippets.Count}")]
    public class ReadSnippets
    {
        public string Directory { get; }
        public IReadOnlyList<Snippet> Snippets { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }
        public IReadOnlyList<Snippet> SnippetsInError { get; }

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