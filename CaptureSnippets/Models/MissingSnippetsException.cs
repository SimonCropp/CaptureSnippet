using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public class MissingSnippetsException : Exception
    {
        public readonly IEnumerable<MissingSnippet> MissingSnippets;

        public MissingSnippetsException(IEnumerable<MissingSnippet> missingSnippets)
        {
            Guard.AgainstNull(missingSnippets, "missingSnippets");
            MissingSnippets = missingSnippets.ToList();
        }

        public override string ToString()
        {
            return Message;
        }

        public override string Message
        {
            get
            {
                var stringBuilder = new StringBuilder();
                foreach (var snippet in MissingSnippets)
                {
                    stringBuilder.AppendLine(snippet.ToString());
                }
                return stringBuilder.ToString();
            }
        }

    }
}