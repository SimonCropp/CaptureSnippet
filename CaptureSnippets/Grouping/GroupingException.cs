using System;
using System.Collections.Generic;

namespace CaptureSnippets
{
    public class GroupingException : Exception
    {
        public readonly IReadOnlyList<string> Errors;

        /// <summary>
        /// Initialise a new instance of <see cref="GroupingException"/>.
        /// </summary>
        public GroupingException(IReadOnlyList<string> errors)
        {
            Guard.AgainstNull(errors, nameof(errors));
            Errors = errors;
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
                builder.AppendLine("Errors occurred Grouping:\r\n");
                foreach (var error in Errors)
                {
                    builder.AppendLine(error);
                }
                return StringBuilderCache.GetStringAndRelease(builder);
            }
        }
    }
}