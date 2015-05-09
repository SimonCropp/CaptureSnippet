using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetError
    {

        public ReadSnippetError(Version version, string key, int line, string file, string message)
        {
            Guard.AgainstNegativeAndZero(line, "line");
            Guard.AgainstNullAndEmpty(key,"key");
            Guard.AgainstNullAndEmpty(message, "message");
            Version = version;
            Key = key;
            Line = line;
            File = file;
            Message = message;
            Version = version;
        }

        public readonly Version Version;
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
                if (Version != Version.ExplicitNull)
                {
                    stringBuilder.AppendFormat(" Version: {0}.", Version);
                }
                else
                {
                    stringBuilder.Append(" Version: ExplicitNull.");
                }
            }
            return stringBuilder.ToString();
        }
    }
}