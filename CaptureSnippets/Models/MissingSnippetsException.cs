using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    public class MissingSnippetsException : Exception
    {
        public List<MissingSnippet> MissingSnippets { get; private set; }

        public MissingSnippetsException(List<MissingSnippet> missingSnippets)
            : base(ToMessage(missingSnippets))
        {
            MissingSnippets = missingSnippets;
        }

        public override string ToString()
        {
            return Message;
        }

        static string ToMessage(List<MissingSnippet> missingSnippets)
        {
            var stringBuilder = new StringBuilder();
            foreach (var snippet in missingSnippets)
            {
                stringBuilder.AppendLine(snippet.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}