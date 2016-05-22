using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Extensions
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

    public static bool StartsWithLetter(this string value)
    {
        return char.IsLetter(value, 0);
    }

    public static string RemoveWhitespace(this string input)
    {
        var builder = new StringBuilder();
        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                continue;
            }
            builder.Append(c);
        }
        return builder.ToString();
    }

    public static string TrimBackCommentChars(this string input)
    {
        for (var index = input.Length - 1; index >= 0; index--)
        {
            var ch = input[index];
            if (char.IsLetterOrDigit(ch) || ch == ']' || ch == ' ' || ch == ')')
            {
                return input.Substring(0, index + 1);
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

    public static string TrimNonCharacters(this string line)
    {
        var array = line.SkipWhile(c => !char.IsLetterOrDigit(c))
            .Reverse()
            .SkipWhile(c => !char.IsLetterOrDigit(c))
            .Reverse().ToArray();
        return new string(array);
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

}