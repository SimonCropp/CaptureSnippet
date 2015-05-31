using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{

    /// <summary>
    /// Thrown for then <see cref="MissingSnippets"/> exist.
    /// </summary>
    public class MissingSnippetsException : Exception
    {
        /// <summary>
        /// The snippets that were requested but not found.
        /// </summary>
        public readonly IEnumerable<MissingSnippet> MissingSnippets;

        /// <summary>
        /// Initialise a new insatnce of <see cref="MissingSnippetsException"/>.
        /// </summary>
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