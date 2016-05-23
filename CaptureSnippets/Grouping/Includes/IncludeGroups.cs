using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of an <see cref="SnippetGrouper.Group"/> method.
    /// </summary>
    public class IncludeGroups : IEnumerable<IncludeGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippets"/>.
        /// </summary>
        public IncludeGroups(IReadOnlyList<IncludeGroup> snippets, IReadOnlyList<string> errors)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(errors, "errors");
            Groups = snippets;
            Errors = errors;
        }

        /// <summary>
        /// The full list of includes.
        /// </summary>
        public readonly IReadOnlyList<IncludeGroup> Groups;

        /// <summary>
        /// Any errors found in the parsing of includes.
        /// </summary>
        public readonly IReadOnlyList<string> Errors;

        /// <summary>
        /// Enumerates through the <see cref="Groups"/> but will first throw an exception if there are any errors in <see cref="Errors"/>.
        /// </summary>
        public IEnumerator<IncludeGroup> GetEnumerator()
        {
            if (Errors.Any())
            {
                throw new GroupingException(Errors);
            }
            return Groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}