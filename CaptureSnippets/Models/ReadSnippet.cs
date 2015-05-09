namespace CaptureSnippets
{
    /// <summary>
    /// A sub item of <see cref="ReadSnippets"/>.
    /// </summary>
    public class ReadSnippet
    {
        public ReadSnippet(int startLine, int endLine, string value, string key, string language, string file, Version version)
        {
            StartLine = startLine;
            EndLine = endLine;
            Value = value;
            Key = key;
            Language = language;
            File = file;
            Version = version;
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
        /// The key used to identify the snippet
        /// </summary>
        public readonly string Key;
        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public readonly string Language;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public readonly string File;
        /// <summary>
        /// The <see cref="Version"/> that was inferred for the snippet.
        /// </summary>
        public readonly Version Version;
    }
}