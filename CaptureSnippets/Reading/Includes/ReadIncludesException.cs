using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// Thrown when there are <see cref="ReadInclude"/>s in error.
    /// </summary>
    public class ReadIncludesException : Exception
    {
        /// <summary>
        /// The list of <see cref="ReadInclude"/>s that are in error.
        /// </summary>
        public readonly IReadOnlyList<ReadInclude> ReadIncludeErrors;

        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippetsException"/>.
        /// </summary>
        public ReadIncludesException(IReadOnlyList<ReadInclude> readIncludeErrors)
        {
            Guard.AgainstNull(readIncludeErrors, "readIncludeErrors");
            ReadIncludeErrors = readIncludeErrors;
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
                foreach (var include in ReadIncludeErrors)
                {
                    stringBuilder.AppendLine(include.ToString());
                }
                return stringBuilder.ToString();
            }
        }
    }
}