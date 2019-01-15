using System;
using CaptureSnippets;

static class StartEndTester
{
    static Func<string, bool> isEndCode = s => IsEndCode(s);
    static Func<string, bool> isEndRegion = s => IsEndRegion(s);

    internal static bool IsStart(string trimmedLine, out string version, out string currentKey, out Func<string, bool> endFunc)
    {
        if (IsStartCode(trimmedLine, out currentKey, out version))
        {
            endFunc = isEndCode;
            return true;
        }
        if (IsStartRegion(trimmedLine, out currentKey, out version))
        {
            endFunc = isEndRegion;
            return true;
        }
        endFunc = null;
        return false;
    }

    static bool IsEndRegion(string line)
    {
        return line.IndexOf("#endregion", StringComparison.Ordinal) >= 0;
    }

    static bool IsEndCode(string line)
    {
        return line.IndexOf("endcode", StringComparison.Ordinal) >= 0;
    }

    internal static bool IsStartRegion(string line, out string key, out string version)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = version = null;
            return false;
        }
        var substring = line.Substring(8);
        return TryExtractParts(out key, out version, substring, line);
    }

    internal static bool IsStartCode(string line, out string key, out string version)
    {
        var startCodeIndex = line.IndexOf("startcode ", StringComparison.Ordinal);
        if (startCodeIndex == -1)
        {
            key = version = null;
            return false;
        }
        var startIndex = startCodeIndex + 10;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(out key, out version, substring, line);
    }

    static bool TryExtractParts(out string key, out string version, string substring, string line)
    {
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new SnippetReadingException($"No Key could be derived. Line: '{line}'.");
        }
        key = split[0];
        KeyValidator.ValidateKeyDoesNotStartOrEndWithSymbol(key);
        if (split.Length == 1)
        {
            version = null;
            return true;
        }
        version = split[1];
        if (split.Length == 2)
        {
            return true;
        }

        throw new SnippetReadingException($"Too many parts. Line: '{line}'.");
    }
}