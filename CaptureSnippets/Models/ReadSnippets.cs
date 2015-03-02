using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of an <see cref="SnippetExtractor"/> From methods.
    /// </summary>
    public class ReadSnippets : IEnumerable<ReadSnippet>
    {

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public List<ReadSnippet> Snippets = new List<ReadSnippet>();

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public List<ReadSnippetError> Errors = new List<ReadSnippetError>();

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