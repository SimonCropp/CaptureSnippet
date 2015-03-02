using System.Text;

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

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("Missing Snippet. ");
            stringBuilder.AppendFormat(" Line: {0}.", Line);
            stringBuilder.AppendFormat(" Key: '{0}'.", Key);
            return stringBuilder.ToString();
        }
    }
}