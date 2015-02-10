namespace CaptureSnippets
{
    /// <summary>
    /// A sub item of <see cref="ReadSnippets"/>.
    /// </summary>
    public class ReadSnippet
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
        /// The key used to identify the snippet
        /// </summary>
        public string Key;
        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public string Language;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public string File;
        /// <summary>
        /// The <see cref="Version"/> that was inferred for the snippet.
        /// </summary>
        public Version Version;
    }
}