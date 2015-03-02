using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippetsException : Exception
    {
        public List<ReadSnippetError> ReadSnippetErrors { get; private set; }

        public ReadSnippetsException(List<ReadSnippetError> readSnippetErrors)
            : base(ToMessage(readSnippetErrors))
        {
            ReadSnippetErrors = readSnippetErrors;
        }

        public override string ToString()
        {
            return Message;
        }

        static string ToMessage(List<ReadSnippetError> readSnippetErrors)
        {
            var stringBuilder = new StringBuilder();
            foreach (var snippet in readSnippetErrors)
            {
                stringBuilder.AppendLine(snippet.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}