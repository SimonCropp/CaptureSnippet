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

        /// <summary>
        /// Converts <see cref="ProcessResult.MissingSnippets"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this ProcessResult processResult)
        {
            if (!processResult.MissingSnippets.Any())
            {
                return "";
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("## Missing snippets\r\n");
            foreach (var error in processResult.MissingSnippets)
            {
                stringBuilder.AppendLine(string.Format(" * Key:'{0}' Line:'{1}'", error.Key, error.Line));
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }


        /// <summary>
        /// If any errors exist in <see cref="ProcessResult.MissingSnippets"/> they are concatenated and an exception is thrown.
        /// </summary>
        public static void ThrowIfErrors(this ProcessResult processResult)
        {
            if (processResult.MissingSnippets.Any())
            {
                var stringBuilder = new StringBuilder();
                foreach (var snippet in processResult.MissingSnippets)
                {
                    stringBuilder.AppendFormat("Key: {0}, Line: {1}", snippet.Key, snippet.Line);
                    stringBuilder.AppendLine();
                }
                throw new Exception("Missing snippets \r\n" + stringBuilder);
            }
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