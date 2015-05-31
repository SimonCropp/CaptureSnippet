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
        /// <summary>
        /// Initialise a new insatnce of <see cref="CachedSnippets"/>.
        /// </summary>
        public CachedSnippets(IEnumerable<SnippetGroup> snippetGroups, long ticks, IEnumerable<ReadSnippetError> readingErrors, IEnumerable<string> groupingErrors)
        {
            Guard.AgainstNull(snippetGroups, "snippetGroups");
            Guard.AgainstNull(readingErrors, "readingErrors");
            Guard.AgainstNull(groupingErrors, "groupingErrors");
            Guard.AgainstNegativeAndZero(ticks, "ticks");
            SnippetGroups = snippetGroups.ToList();
            ReadingErrors = readingErrors.ToList();
            Ticks = ticks;
            GroupingErrors = groupingErrors;
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
        /// Errors that occured as part of the grouping.
        /// </summary>
        public readonly IEnumerable<string> GroupingErrors;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public readonly IEnumerable<ReadSnippetError> ReadingErrors;
        
        /// <summary>
        /// Enumerates through the <see cref="SnippetGroups"/> but will first throw an exception if there are any errors in <see cref="ReadingErrors"/>.
        /// </summary>
        public IEnumerator<SnippetGroup> GetEnumerator()
        {
            if (ReadingErrors.Any())
            {
                throw new ReadSnippetsException(ReadingErrors);
            }
            if (GroupingErrors.Any())
            {
                throw new GroupingException(GroupingErrors);
            }
            return SnippetGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}