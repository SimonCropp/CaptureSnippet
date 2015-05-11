using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    static class StringExtensions
    {
        public static IEnumerable<string> TrimIndentation(this IEnumerable<string> snippetLines)
        {
            string initialPadding = null;
            foreach (var line in snippetLines)
            {
                if (initialPadding == null)
                {
                    initialPadding = new string(line.TakeWhile(char.IsWhiteSpace).ToArray());
                }
                yield return line.RemoveStart(initialPadding);
            }
        }
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        public static string TrimNonCharacters(this string line)
        {
            var array = line.SkipWhile(c => !char.IsLetterOrDigit(c))
                .Reverse()
                .SkipWhile(c => !char.IsLetterOrDigit(c))
                .Reverse().ToArray();
            return new string(array);
        }

        public static string Reverse(this string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static IEnumerable<string> ExcludeEmptyPaddingLines(this IEnumerable<string> snippetLines)
        {
            var list = snippetLines.ToList();
            while (list.Count > 0 && list[0].IsWhiteSpace())
            {
                list.RemoveAt(0);
            }
            while (list.Count > 0 && list[list.Count - 1].IsWhiteSpace())
            {
                list.RemoveAt(list.Count - 1);
            }
            return list;
        }

        public static bool IsWhiteSpace(this string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }

        public static string RemoveStart(this string target, int count)
        {
            return target.Substring(count, target.Length - count);
        }

        public static string RemoveStart(this string target, string toRemove)
        {
            if (target.StartsWith(toRemove))
            {
                var count = toRemove.Length;
                return target.RemoveStart(count);
            }
            return target;
        }

        public static string TrimTrailingNewLine(this string target)
        {
            return target.TrimEnd('\r', '\n'); 
        }

        public static bool IsMdCodeDelimiter(this string line)
        {
            return line.StartsWith("```");
        }

    }
}