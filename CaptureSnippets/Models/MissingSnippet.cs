namespace CaptureSnippets
{
    /// <summary>
    /// Part of <see cref="ProcessResult"/>.
    /// </summary>
    public class MissingSnippet
    {
        /// <summary>
        /// The key of the missing snippet.
        /// </summary>
        public string Key;
        /// <summary>
        /// The line number in the input text where the snippet was expected to be injected.
        /// </summary>
        public int Line;
    }
}