using System;

static class StartEndTester
{
    static Func<string, bool> isEndCode = s => IsEndCode(s);
    static Func<string, bool> isEndRegion = s => IsEndRegion(s);

    internal static bool IsStart(string trimmedLine, out string currentKey, out Func<string, bool> endFunc)
    {
        if (IsStartCode(trimmedLine, out currentKey))
        {
            endFunc = isEndCode;
            return true;
        }
        if (IsStartRegion(trimmedLine, out currentKey))
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

    internal static bool IsStartRegion(string line, out string key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = null;
            return false;
        }
        var substring = line.Substring(8);
        return TryExtractParts(out key, substring, line);
    }

    internal static bool IsStartCode(string line, out string key)
    {
        var startCodeIndex = line.IndexOf("startcode ", StringComparison.Ordinal);
        if (startCodeIndex == -1)
        {
            key  = null;
            return false;
        }
        var startIndex = startCodeIndex + 10;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(out key, substring, line);
    }

    static bool TryExtractParts(out string key, string substring, string line)
    {
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new Exception($"No Key could be derived. Line: '{line}'.");
        }
        key = split[0];
        ValidateKeyDoesNotStartOrEndWithSymbol(key);
        if (split.Length == 1)
        {
            return true;
        }

        throw new Exception($"Too many parts. Line: '{line}'.");
    }

    static void ValidateKeyDoesNotStartOrEndWithSymbol(string key)
    {
        if (char.IsLetterOrDigit(key, 0) && char.IsLetterOrDigit(key, key.Length - 1))
        {
            return;
        }
        throw new Exception($"Key should not start or end with symbols. Key: {key}");
    }
}