using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="MarkdownProcessor"/> Apply methods.
    /// </summary>
    public class ProcessResult : IEnumerable<SnippetGroup>
    {

        public ProcessResult(IEnumerable<SnippetGroup> usedSnippets,IEnumerable<MissingSnippet> missingSnippets)
        {
            Guard.AgainstNull(usedSnippets, "usedSnippets");
            Guard.AgainstNull(missingSnippets, "missingSnippets");
            UsedSnippets = usedSnippets.ToList();
            MissingSnippets = missingSnippets.ToList();
        }

        /// <summary>
        ///   List of all snippets that the markdown file used. 
        /// </summary>
        public readonly IEnumerable<SnippetGroup> UsedSnippets;
        /// <summary>
        ///   List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public readonly IEnumerable<MissingSnippet> MissingSnippets;


        /// <summary>
        /// Enumerates through the <see cref="UsedSnippets"/> but will first throw an exception if there are any errors in <see cref="MissingSnippets"/>.
        /// </summary>
        public IEnumerator<SnippetGroup> GetEnumerator()
        {
            this.ThrowIfErrors();
            return UsedSnippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}