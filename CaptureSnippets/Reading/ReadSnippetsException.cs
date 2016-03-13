using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetsException : Exception
    {
        public readonly IReadOnlyList<ReadSnippet> ReadSnippetErrors;

        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippetsException"/>.
        /// </summary>
        public ReadSnippetsException(IReadOnlyList<ReadSnippet> readSnippetErrors)
        {
            Guard.AgainstNull(readSnippetErrors, "readSnippetErrors");
            ReadSnippetErrors = readSnippetErrors;
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
                foreach (var snippet in ReadSnippetErrors)
                {
                    stringBuilder.AppendLine(snippet.ToString());
                }
                return stringBuilder.ToString();
            }
        }
    }
}