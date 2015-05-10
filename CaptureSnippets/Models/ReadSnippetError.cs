using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Represents an error encountered by <see cref="SnippetExtractor"/>.
    /// </summary>
    public class ReadSnippetError
    {

        /// <summary>
        /// Initialise a new insatnce of <see cref="ReadSnippetError"/>.
        /// </summary>
        public ReadSnippetError(VersionRange version, string key, int line, string file, string message)
        {
            Guard.AgainstNegativeAndZero(line, "line");
            Guard.AgainstNullAndEmpty(key, "key");
            Guard.AgainstNullAndEmpty(message, "message");
            Version = version;
            Key = key;
            Line = line;
            File = file;
            Message = message;
        }

        public readonly VersionRange Version;
        public readonly string Key;
        public readonly int Line;
        public readonly string File;
        public readonly string Message;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(string.Format("{0}.", Message));
            if (File != null)
            {
                stringBuilder.AppendFormat(" File: '{0}'.", File);
            }
            stringBuilder.AppendFormat(" Line: {0}.", Line);
            stringBuilder.AppendFormat(" Key: '{0}'.", Key);
            if (Version != null)
            {
                if (Version.Equals(VersionRange.All))
                {
                    stringBuilder.Append(" Version: All.");
                }
                else
                {
                    stringBuilder.AppendFormat(" Version: '{0}'.", Version);
                }
            }
            return stringBuilder.ToString();
        }
    }
}