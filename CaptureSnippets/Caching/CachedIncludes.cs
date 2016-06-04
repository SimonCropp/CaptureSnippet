using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{

    /// <summary>
    /// The information cached by <see cref="CachedIncludeExtractor"/>.
    /// </summary>
    public class CachedIncludes : IEnumerable<IncludeGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="CachedSnippets"/>.
        /// </summary>
        public CachedIncludes(IReadOnlyList<IncludeGroup> includeGroups, long ticks, IReadOnlyList<ReadInclude> readingErrors, IReadOnlyList<string> groupingErrors)
        {
            Guard.AgainstNull(includeGroups, "includeGroups");
            Guard.AgainstNull(readingErrors, "readingErrors");
            Guard.AgainstNull(groupingErrors, "groupingErrors");
            Guard.AgainstNegativeAndZero(ticks, "ticks");
            IncludeGroups = includeGroups;
            ReadingErrors = readingErrors;
            Ticks = ticks;
            GroupingErrors = groupingErrors;
        }

        /// <summary>
        /// The grouped includes from the passed in directory.
        /// </summary>
        public readonly IReadOnlyList<IncludeGroup> IncludeGroups;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public readonly long Ticks;

        /// <summary>
        /// Errors that occurred as part of the grouping.
        /// </summary>
        public readonly IReadOnlyList<string> GroupingErrors;

        /// <summary>
        /// Any errors found in the parsing of includes.
        /// </summary>
        public readonly IReadOnlyList<ReadInclude> ReadingErrors;

        /// <summary>
        /// Enumerates through the <see cref="IncludeGroups"/> but will first throw an exception if there are any errors in <see cref="ReadingErrors"/>.
        /// </summary>
        public IEnumerator<IncludeGroup> GetEnumerator()
        {
            if (ReadingErrors.Any())
            {
                throw new ReadIncludesException(ReadingErrors);
            }
            if (GroupingErrors.Any())
            {
                throw new GroupingException(GroupingErrors);
            }
            return IncludeGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}