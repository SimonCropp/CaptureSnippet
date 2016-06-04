using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of <see cref="MarkdownProcessor"/> Apply methods.
    /// </summary>
    public class ProcessResult
    {

        public ProcessResult(IReadOnlyList<SnippetGroup> usedSnippets, IReadOnlyList<IncludeGroup> usedIncludes, IReadOnlyList<MissingKey> missing)
        {
            Guard.AgainstNull(usedSnippets, nameof(usedSnippets));
            Guard.AgainstNull(usedIncludes, nameof(usedIncludes));
            Guard.AgainstNull(missing, nameof(missing));
            UsedSnippets = usedSnippets;
            UsedIncludes = usedIncludes;
            Missing = missing;
        }

        /// <summary>
        ///   List of all snippets that the markdown file used.
        /// </summary>
        public readonly IReadOnlyList<SnippetGroup> UsedSnippets;

        /// <summary>
        ///   List of all includes that the markdown file used.
        /// </summary>
        public readonly IReadOnlyList<IncludeGroup> UsedIncludes;

        /// <summary>
        ///   List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public readonly IReadOnlyList<MissingKey> Missing;

    }
}