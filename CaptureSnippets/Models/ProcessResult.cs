using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="MarkdownProcessor"/> Apply methods.
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        ///   List of all snippets that the markdown file used. 
        /// </summary>
        public List<SnippetGroup> UsedSnippets = new List<SnippetGroup>();
        /// <summary>
        ///   List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public List<MissingSnippet> MissingSnippet = new List<MissingSnippet>();

    }
}