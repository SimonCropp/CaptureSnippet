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
        public ReadSnippetError(VersionRange version, string key, int line, string path, string message, string package)
        {
            Guard.AgainstNegativeAndZero(line, "line");
            Guard.AgainstNullAndEmpty(key, "key");
            Guard.AgainstNullAndEmpty(message, "message");
            Version = version;
            Package = package;
            Key = key;
            Line = line;
            Path = path;
            Message = message;
        }

        /// <summary>
        /// The snippet version if one could be detected before the error was encountered.
        /// </summary>
        public readonly VersionRange Version;
        /// <summary>
        /// The snippet package if one could be detected before the error was encountered.
        /// </summary>
        public readonly string Package;
        /// <summary>
        /// The key used to identify the snippet
        /// </summary>
        public readonly string Key;
        /// <summary>
        /// The line the error was encountered.
        /// </summary>
        public readonly int Line;
        /// <summary>
        /// The file path the error was encountered in.
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// A description of the error.
        /// </summary>
        public readonly string Message;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder($"{Message}.");
            if (Path != null)
            {
                stringBuilder.AppendFormat(" File: '{0}'.", Path);
            }
            stringBuilder.AppendFormat(" Line: {0}.", Line);
            stringBuilder.AppendFormat(" Key: '{0}'.", Key);
            if (Package != null)
            {
                stringBuilder.AppendFormat(" Package: '{0}'.", Package);
            }
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