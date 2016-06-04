using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{

    /// <summary>
    /// Thrown for when <see cref="MissingSnippets"/> exist.
    /// </summary>
    public class MissingSnippetsException : Exception
    {
        /// <summary>
        /// The snippets that were requested but not found.
        /// </summary>
        public readonly IReadOnlyList<MissingKey> MissingSnippets;

        /// <summary>
        /// Initialise a new instance of <see cref="MissingSnippetsException"/>.
        /// </summary>
        public MissingSnippetsException(IReadOnlyList<MissingKey> missingSnippets)
        {
            Guard.AgainstNull(missingSnippets, "missingSnippets");
            MissingSnippets = missingSnippets;
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