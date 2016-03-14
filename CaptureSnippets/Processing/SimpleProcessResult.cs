using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="GroupedMarkdownProcessor"/> Apply methods.
    /// </summary>
    public class SimpleProcessResult : IEnumerable<ReadSnippet>
    {

        public SimpleProcessResult(IReadOnlyList<ReadSnippet> usedSnippets, IReadOnlyList<MissingSnippet> missingSnippets)
        {
            Guard.AgainstNull(usedSnippets, "usedSnippets");
            Guard.AgainstNull(missingSnippets, "missingSnippets");
            UsedSnippets = usedSnippets;
            MissingSnippets = missingSnippets;
        }

        /// <summary>
        ///   List of all snippets that the markdown file used. 
        /// </summary>
        public readonly IReadOnlyList<ReadSnippet> UsedSnippets;
        /// <summary>
        ///   List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public readonly IReadOnlyList<MissingSnippet> MissingSnippets;


        /// <summary>
        /// Enumerates through the <see cref="UsedSnippets"/> but will first throw an exception if there are any errors in <see cref="MissingSnippets"/>.
        /// </summary>
        public IEnumerator<ReadSnippet> GetEnumerator()
        {
            if (MissingSnippets.Any())
            {
                throw new MissingSnippetsException(MissingSnippets);
            }
            return UsedSnippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}