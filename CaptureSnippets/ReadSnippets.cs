using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public class ReadSnippets
    {
        public List<ReadSnippet> Snippets = new List<ReadSnippet>();

        public List<string> Errors = new List<string>();

        public string ErrorsAsMarkdown
        {
            get
            {
                if (!Errors.Any())
                {
                    return "";
                }
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("## Snippet errors\r\n");
                foreach (var error in Errors)
                {
                    stringBuilder.AppendLine(" * " + error);
                }
                stringBuilder.AppendLine();
                return stringBuilder.ToString();
            }
        }

        public void ThrowIfErrors()
        {
            if (Errors.Any())
            {
                var error = String.Join(Environment.NewLine, Errors);
                throw new Exception(error);
            }
        }
    }
}