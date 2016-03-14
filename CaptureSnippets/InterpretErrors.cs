using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// Extension method to convert various error cases.
    /// </summary>
    public static class InterpretErrors
    {

        /// <summary>
        /// Converts <see cref="ReadSnippets.GetSnippetsInError"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this ReadSnippets readSnippets)
        {
            Guard.AgainstNull(readSnippets, "readSnippets");
            return ErrorsAsMarkdown(readSnippets.GetSnippetsInError());
        }

        /// <summary>
        /// Converts <see cref="CachedSnippets.ReadingErrors"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this CachedSnippets cachedSnippets)
        {
            Guard.AgainstNull(cachedSnippets, "cachedSnippets");
            return ErrorsAsMarkdown(cachedSnippets.ReadingErrors);
        }

        static string ErrorsAsMarkdown(IEnumerable<ReadSnippet> errors)
        {
            var readSnippetErrors = errors.ToList();
            if (!readSnippetErrors.Any())
            {
                return "";
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("## Snippet errors\r\n");
            foreach (var error in readSnippetErrors)
            {
                stringBuilder.AppendLine(" * " + error);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Converts <see cref="GroupedProcessResult.MissingSnippets"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this GroupedProcessResult groupedProcessResult)
        {
            Guard.AgainstNull(groupedProcessResult, "GroupedProcessResult");
            var missingSnippets = groupedProcessResult.MissingSnippets.ToList();
            if (!missingSnippets.Any())
            {
                return "";
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("## Missing snippets\r\n");
            foreach (var error in missingSnippets)
            {
                stringBuilder.AppendLine($" * Key:'{error.Key}' Line:'{error.Line}'");
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }
    }
}