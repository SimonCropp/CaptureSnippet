namespace CaptureSnippets
{
    /// <summary>
    /// A snippet after it has been grouped by <see cref="SnippetGrouper"/>.
    /// </summary>
    /// <remarks>Note that <see cref="ReadSnippet.Version"/> and <see cref="ReadSnippet.Key"/> are not included since they can be infered by the grouping structure.</remarks>
    public class Snippet
    {
        /// <summary>
        /// The line the snippets started on
        /// </summary>
        public int StartLine;
        /// <summary>
        /// The line the snippet ended on.
        /// </summary>
        public int EndLine;
        /// <summary>
        /// The contents of the snippet
        /// </summary>
        public string Value;
        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public string Language;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public string File;



    }
}