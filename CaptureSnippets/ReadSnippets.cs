using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// The result of an <see cref="SnippetExtractor"/> From methods.
    /// </summary>
    public class ReadSnippets : IEnumerable<ReadSnippet>
    {

        /// <summary>
        /// The full list of snippets.
        /// </summary>
        public List<ReadSnippet> Snippets = new List<ReadSnippet>();

        /// <summary>
        /// Any errors found in the parsing of snippets.
        /// </summary>
        public List<string> Errors = new List<string>();

        /// <summary>
        /// Converts <see cref="Errors"/> to a markdown string.
        /// </summary>
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

        /// <summary>
        /// If any errors exist in <see cref="Errors"/> they are concatenated and an exception is thrown.
        /// </summary>
        public void ThrowIfErrors()
        {
            if (Errors.Any())
            {
                var error = String.Join(Environment.NewLine, Errors);
                throw new Exception(error);
            }
        }
        /// <summary>
        /// Enumerates through the <see cref="Snippets"/> but will first throw an exception if there are any errors.
        /// </summary>
        public IEnumerator<ReadSnippet> GetEnumerator()
        {
            ThrowIfErrors();
            return Snippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}