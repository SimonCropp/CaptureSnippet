using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetsException : Exception
    {
        public readonly IEnumerable<ReadSnippetError> ReadSnippetErrors;

        /// <summary>
        /// Initialise a new insatnce of <see cref="ReadSnippetsException"/>.
        /// </summary>
        public ReadSnippetsException(IEnumerable<ReadSnippetError> readSnippetErrors)
        {
            Guard.AgainstNull(readSnippetErrors, "readSnippetErrors");
            ReadSnippetErrors = readSnippetErrors.ToList();
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