namespace CaptureSnippets
{
    /// <summary>
    /// A snippet after it has been grouped by <see cref="SnippetGrouper"/>.
    /// </summary>
    /// <remarks>Note that <see cref="ReadSnippet.Version"/> and <see cref="ReadSnippet.Key"/> are not included since they can be infered by the grouping structure.</remarks>
    public class SnippetSource
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetSource"/>.
        /// </summary>
        public SnippetSource(int startLine, int endLine, string file)
        {
            Guard.AgainstNegativeAndZero(startLine, "startLine");
            Guard.AgainstNegativeAndZero(endLine, "endLine");
            File = file;
            StartLine = startLine;
            EndLine = endLine;
        }

        /// <summary>
        /// The line the snippets started on
        /// </summary>
        public readonly int StartLine;
        /// <summary>
        /// The line the snippet ended on.
        /// </summary>
        public readonly int EndLine;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public readonly string File;
    }
}