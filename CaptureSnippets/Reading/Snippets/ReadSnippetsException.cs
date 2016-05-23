using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// Thrown when there are <see cref="ReadSnippet"/>s in error.
    /// </summary>
    public class ReadSnippetsException : Exception
    {
        /// <summary>
        /// The list of <see cref="ReadSnippet"/>s that are in error.
        /// </summary>
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