using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{

    /// <summary>
    /// The information cached by <see cref="CachedSnippetExtractor"/>.
    /// </summary>
    public class CachedSnippets : IEnumerable<SnippetGroup>
    {
        /// <summary>
        /// The grouped snippets from the passed in directory.
        /// </summary>
        public List<SnippetGroup> SnippetGroups;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public long Ticks;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public List<ReadSnippetError> Errors = new List<ReadSnippetError>();
        
        /// <summary>
        /// Enumerates through the <see cref="SnippetGroups"/> but will first throw an exception if there are any errors in <see cref="Errors"/>.
        /// </summary>
        public IEnumerator<SnippetGroup> GetEnumerator()
        {
            this.ThrowIfErrors();
            return SnippetGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}