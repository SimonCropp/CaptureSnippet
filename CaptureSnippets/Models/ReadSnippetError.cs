using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetError
    {
        public Version Version
        {
            get { return version; }
            set
            {
                Guard.AgainstNull(value,"value");
                version = value;
            }
        }

        public string Key;
        public int Line;
        public string File;
        public string Message;
        Version version;

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