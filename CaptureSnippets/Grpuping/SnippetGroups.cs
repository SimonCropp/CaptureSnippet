using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of an <see cref="SnippetGrouper.Group"/> method.
    /// </summary>
    public class SnippetGroups : IEnumerable<SnippetGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippets"/>.
        /// </summary>
        public SnippetGroups(IReadOnlyList<SnippetGroup> snippets, IReadOnlyList<string> errors)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(errors, "errors");
            Groups = snippets;
            Errors = errors;
        }

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public readonly IReadOnlyList<SnippetGroup> Groups;

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public readonly IReadOnlyList<string> Errors;

        /// <summary>
        /// Enumerates through the <see cref="Groups"/> but will first throw an exception if there are any errors in <see cref="Errors"/>.
        /// </summary>
        public IEnumerator<SnippetGroup> GetEnumerator()
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