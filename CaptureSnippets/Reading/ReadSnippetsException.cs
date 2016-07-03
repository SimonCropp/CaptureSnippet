using System;
using System.Collections.Generic;

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
            Guard.AgainstNull(readSnippetErrors, nameof(readSnippetErrors));
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
                var builder = StringBuilderCache.Acquire();
                foreach (var snippet in ReadSnippetErrors)
                {
                    builder.AppendLine(snippet.ToString());
                }
                return StringBuilderCache.GetStringAndRelease(builder);
            }
        }
    }
}