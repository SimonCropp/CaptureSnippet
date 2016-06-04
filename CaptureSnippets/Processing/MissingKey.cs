using System.Diagnostics;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// Part of <see cref="ProcessResult"/>.
    /// </summary>
    [DebuggerDisplay("Key={Key}, Line={Line}")]
    public class MissingKey
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MissingKey"/>.
        /// </summary>
        public MissingKey(string key, int line)
        {
            Guard.AgainstNullAndEmpty(key,"key");
            Guard.AgainstNegativeAndZero(line, "line");
            Key = key;
            Line = line;
        }
        /// <summary>
        /// The key of the missing snippet or include.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The line number in the input text where the snippet or include was expected to be injected.
        /// </summary>
        public readonly int Line;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("Missing Snippet or Include. ");
            stringBuilder.AppendFormat(" Line: {0}.", Line);
            stringBuilder.AppendFormat(" Key: '{0}'.", Key);
            return stringBuilder.ToString();
        }
    }
}