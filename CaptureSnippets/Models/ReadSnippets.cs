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
        /// Initialise a new insatnce of <see cref="ReadSnippets"/>.
        /// </summary>
        public ReadSnippets(IEnumerable<ReadSnippet> snippets, IEnumerable<ReadSnippetError> errors)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(errors, "errors");
            Snippets = snippets.ToList();
            Errors = errors.ToList();
        }

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public readonly IEnumerable<ReadSnippet> Snippets;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public readonly IEnumerable<ReadSnippetError> Errors ;

        /// <summary>
        /// Enumerates through the <see cref="Snippets"/> but will first throw an exception if there are any errors in <see cref="Errors"/>.
        /// </summary>
        public IEnumerator<ReadSnippet> GetEnumerator()
        {
            this.ThrowIfErrors();
            return Snippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}