using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="MarkdownProcessor"/> Apply methods.
    /// </summary>
    public class ProcessResult : IEnumerable<SnippetGroup>
    {
        /// <summary>
        ///   List of all snippets that the markdown file used. 
        /// </summary>
        public List<SnippetGroup> UsedSnippets = new List<SnippetGroup>();
        /// <summary>
        ///   List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public List<MissingSnippet> MissingSnippets = new List<MissingSnippet>();


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