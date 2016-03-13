using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of an <see cref="SnippetExtractor"/> From methods.
    /// </summary>
    public class ReadSnippets : IEnumerable<ReadSnippet>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippets"/>.
        /// </summary>
        public ReadSnippets(IReadOnlyList<ReadSnippet> snippets)
        {
            Guard.AgainstNull(snippets, "snippets");
            Snippets = snippets;
        }

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public readonly IReadOnlyList<ReadSnippet> Snippets;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public IEnumerable<ReadSnippet> GetSnippetsInError()
        {
            return Snippets.Where(x => x.IsInError);
        }

        /// <summary>
        /// Enumerates through the <see cref="Snippets"/> but will first throw an exception if there are any errors in <see cref="Errors"/>.
        /// </summary>
        public IEnumerator<ReadSnippet> GetEnumerator()
        {
            var snippetsInError = GetSnippetsInError().ToList();
            if (snippetsInError.Any())
            {
                throw new ReadSnippetsException(snippetsInError);
            }
            return Snippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}