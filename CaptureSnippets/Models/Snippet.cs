namespace CaptureSnippets
{
    /// <summary>
    /// A snippet after it has been grouped by <see cref="SnippetGrouper"/>.
    /// </summary>
    public class Snippet
    {
        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public string Language;
        /// <summary>
        /// The contents of the snippet.
        /// </summary>
        public string Value;
        /// <summary>
        /// The underlying <see cref="ReadSnippet"/> this <see cref="Snippet"/> was grouped from.
        /// </summary>
        public ReadSnippet Source;
    }
}