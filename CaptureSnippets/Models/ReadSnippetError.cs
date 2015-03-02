using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetError
    {
        public Version Version;
        public string Key;
        public int Line;
        public string File;
        public string Message;

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
                stringBuilder.AppendFormat(" Version: {0}.", Version);
            }
            return stringBuilder.ToString();
        }
    }
}