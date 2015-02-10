using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public static class InterpretErrors
    {

        /// <summary>
        /// Converts <see cref="ReadSnippets.Errors"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this ReadSnippets readSnippets)
        {
            return ErrorsAsMarkdown(readSnippets.Errors);
        }

        /// <summary>
        /// Converts <see cref="CachedSnippets.Errors"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this CachedSnippets cachedSnippets)
        {
            return ErrorsAsMarkdown(cachedSnippets.Errors);
        }

        static string ErrorsAsMarkdown(List<string> errors)
        {
            if (!errors.Any())
            {
                return "";
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("## Snippet errors\r\n");
            foreach (var error in errors)
            {
                stringBuilder.AppendLine(" * " + error);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// If any errors exist in <see cref="ReadSnippets.Errors"/> they are concatenated and an exception is thrown.
        /// </summary>
        public static void ThrowIfErrors(this ReadSnippets readSnippets)
        {
            ThrowIfErrors(readSnippets.Errors);
        }

        /// <summary>
        /// If any errors exist in <see cref="CachedSnippets.Errors"/> they are concatenated and an exception is thrown.
        /// </summary>
        public static void ThrowIfErrors(this CachedSnippets cachedSnippets)
        {
            ThrowIfErrors(cachedSnippets.Errors);
        }

        static void ThrowIfErrors(List<string> errors)
        {
            if (errors.Any())
            {
                var error = String.Join(Environment.NewLine, errors);
                throw new Exception(error);
            }
        }
    }
}