using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="IncludeExtractor"/> .
    /// </summary>
    public class ReadIncludes : IEnumerable<ReadInclude>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ReadIncludes"/>.
        /// </summary>
        public ReadIncludes(IReadOnlyList<ReadInclude> includes)
        {
            Guard.AgainstNull(includes, nameof(includes));
            Includes = includes;
        }

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public readonly IReadOnlyList<ReadInclude> Includes;

        /// <summary>
        /// Any errors found in the parsing of includes.
        /// </summary>
        public IEnumerable<ReadInclude> GetIncludesInError()
        {
            return Includes.Where(x => x.IsInError);
        }

        /// <summary>
        /// Enumerates through the <see cref="Includes"/> but will first throw an exception if there are any <see cref="ReadInclude.IsInError"/>.
        /// </summary>
        public IEnumerator<ReadInclude> GetEnumerator()
        {
            var snippetsInError = GetIncludesInError().ToList();
            if (snippetsInError.Any())
            {
                throw new ReadIncludesException(snippetsInError);
            }
            return Includes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}