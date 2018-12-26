using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Snippets.Count}")]
    public class ReadSnippets : IEnumerable<Snippet>
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

        /// <summary>
        /// Enumerates through the <see cref="Snippets" /> but will first throw an exception if there are any <see cref="SnippetsInError" />.
        /// </summary>
        public virtual IEnumerator<Snippet> GetEnumerator()
        {
            if (SnippetsInError.Any())
            {
                throw new Exception("SnippetsInError: " + string.Join(", ", SnippetsInError.Select(x => x.Key)));
            }

            return Snippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}