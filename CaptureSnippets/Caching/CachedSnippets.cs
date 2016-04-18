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
        /// Initialise a new instance of <see cref="CachedSnippets"/>.
        /// </summary>
        public CachedSnippets(IReadOnlyList<SnippetGroup> snippetGroups, long ticks, IReadOnlyList<ReadSnippet> readingErrors, IReadOnlyList<string> groupingErrors)
        {
            Guard.AgainstNull(snippetGroups, "snippetGroups");
            Guard.AgainstNull(readingErrors, "readingErrors");
            Guard.AgainstNull(groupingErrors, "groupingErrors");
            Guard.AgainstNegativeAndZero(ticks, "ticks");
            SnippetGroups = snippetGroups;
            ReadingErrors = readingErrors;
            Ticks = ticks;
            GroupingErrors = groupingErrors;
        }
        /// <summary>
        /// The grouped snippets from the passed in directory.
        /// </summary>
        public readonly IReadOnlyList<SnippetGroup> SnippetGroups;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public readonly long Ticks;

        /// <summary>
        /// Errors that occurred as part of the grouping.
        /// </summary>
        public readonly IReadOnlyList<string> GroupingErrors;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public readonly IReadOnlyList<ReadSnippet> ReadingErrors;

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