using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaptureSnippets;

static class Extensions
{
    public static void TrimEnd(this StringBuilder builder)
    {
        var i = builder.Length - 1;
        for (; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(builder[i]))
            {
                break;
            }
        }

        if (i < builder.Length - 1)
        {
            builder.Length = i + 1;
        }
    }

    public static IReadOnlyList<T> ToReadonlyList<T>(this IEnumerable<T> value)
    {
        return value.ToList();
    }

    public static Dictionary<string, IReadOnlyList<Snippet>> ToDictionary(this IEnumerable<Snippet> value)
    {
        //TODO: throw if mixing
        //if (package == Package.Undefined)
        //{
        //    if (!string.IsNullOrWhiteSpace(targetPackage))
        //    {
        //        throw new Exception("Cannot mix non-empty targetPackage with a snippet that is Package.Undefined.");
        //    }
        //    packageText = "";
        //}
        return value
            .GroupBy(_ => _.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: _ => _.Key,
                elementSelector: _ => _.ToReadonlyList(),
                comparer: StringComparer.OrdinalIgnoreCase);
    }

    public static int LastIndexOfSequence(this string value, char c, int max)
    {
        var index = 0;
        while (true)
        {
            if (index == max)
            {
                return index;
            }
            if (index == value.Length)
            {
                return index;
            }
            var ch = value[index];
            if (c != ch)
            {
                return index;
            }
            index++;
        }
    }

    public static string TrimBackCommentChars(this string input, int startIndex)
    {
        for (var index = input.Length - 1; index >= startIndex; index--)
        {
            var ch = input[index];
            if (char.IsLetterOrDigit(ch) || ch == ']' || ch == ' ' || ch == ')')
            {
                return input.Substring(startIndex,  index + 1 - startIndex);
            }
        }
        return string.Empty;
    }

    public static string[] SplitBySpace(this string substring)
    {
        return substring
            .Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static bool IsWhiteSpace(this string target)
    {
        return string.IsNullOrWhiteSpace(target);
    }

}