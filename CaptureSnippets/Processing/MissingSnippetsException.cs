using System;
using System.Collections.Generic;

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
        public readonly IReadOnlyList<MissingSnippet> MissingSnippets;

        /// <summary>
        /// Initialise a new instance of <see cref="MissingSnippetsException"/>.
        /// </summary>
        public MissingSnippetsException(IReadOnlyList<MissingSnippet> missingSnippets)
        {
            Guard.AgainstNull(missingSnippets, nameof(missingSnippets));
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
                var builder = StringBuilderCache.Acquire();
                foreach (var snippet in MissingSnippets)
                {
                    builder.AppendLine(snippet.ToString());
                }
                return StringBuilderCache.GetStringAndRelease(builder);
            }
        }

    }
}