namespace CaptureSnippets
{
    /// <summary>
    /// A snippet after it has been grouped by <see cref="SnippetGrouper"/>.
    /// </summary>
    /// <remarks>Note that <see cref="ReadSnippet.Version"/> and <see cref="ReadSnippet.Key"/> are not included since they can be infered by the grouping structure.</remarks>
    public class Snippet
    {
        /// <summary>
        /// Initialise a new insatnce of <see cref="Snippet"/>.
        /// </summary>
        public Snippet(int startLine, int endLine, string value, string language, string file)
        {
            Guard.AgainstNegativeAndZero(startLine, "startLine");
            Guard.AgainstNegativeAndZero(endLine, "endLine");
            Guard.AgainstNull(language, "language");
            File = file;
            StartLine = startLine;
            EndLine = endLine;
            Value = value;
            Language = language;
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
        /// The contents of the snippet
        /// </summary>
        public readonly string Value;
        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public readonly string Language;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public readonly string File;
    }
}