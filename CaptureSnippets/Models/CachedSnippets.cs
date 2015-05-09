using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{

    /// <summary>
    /// The information cached by <see cref="CachedSnippetExtractor"/>.
    /// </summary>
    public class CachedSnippets : IEnumerable<SnippetGroup>
    {
        public CachedSnippets(IEnumerable<SnippetGroup> snippetGroups, long ticks, IEnumerable<ReadSnippetError> errors)
        {
            Guard.AgainstNull(snippetGroups, "snippetGroups");
            Guard.AgainstNull(errors, "errors");
            Guard.AgainstNegativeAndZero(ticks, "ticks");
            SnippetGroups = snippetGroups.ToList();
            Errors = errors.ToList();
            Ticks = ticks;
        }
        /// <summary>
        /// The grouped snippets from the passed in directory.
        /// </summary>
        public readonly IEnumerable<SnippetGroup> SnippetGroups;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public readonly long Ticks;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public readonly IEnumerable<ReadSnippetError> Errors;
        
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