using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public class GroupingException : Exception
    {
        public readonly IEnumerable<string> Errors;

        /// <summary>
        /// Initialise a new insatnce of <see cref="ReadSnippetsException"/>.
        /// </summary>
        public GroupingException(IEnumerable<string> errors)
        {
            Guard.AgainstNull(errors, "errors");
            Errors = errors.ToList();
        }

        public override string ToString()
        {
            return Message;
        }

        public override string Message
        {
            get
            {
                var stringBuilder = new StringBuilder("Errors occured Grouping snippets:\r\n");
                foreach (var error in Errors)
                {
                    stringBuilder.AppendLine(error);
                }
                return stringBuilder.ToString();
            }
        }
    }
}