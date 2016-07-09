using System;

static class StartEndTester
{

    internal static bool IsStart(string trimmedLine, out string suffix1, out string suffix2, out string currentKey, out Func<string, bool> endFunc)
    {
        if (IsStartCode(trimmedLine, out currentKey, out suffix1, out suffix2))
        {
            endFunc = IsEndCode;
            return true;
        }
        if (IsStartRegion(trimmedLine, out currentKey, out suffix1, out suffix2))
        {
            endFunc = IsEndRegion;
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

    internal static bool IsStartRegion(string line, out string key, out string suffix1, out string suffix2)
    {
        if (!line.StartsWith("#region", StringComparison.Ordinal))
        {
            key = suffix2 = suffix1 = null;
            return false;
        }
        var substring = line.Substring(8);
        return TryExtractParts(out key, out suffix1, out suffix2, substring, line);
    }

    internal static bool IsStartCode(string line, out string key, out string suffix1, out string suffix2)
    {
        var startCodeIndex = line.IndexOf("startcode", StringComparison.Ordinal);
        if (startCodeIndex == -1)
        {
            key = suffix2 = suffix1 = null;
            return false;
        }
        var startIndex = startCodeIndex + 10;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(out key, out suffix1, out suffix2, substring, line);
    }

    static bool TryExtractParts(out string key, out string suffix1, out string suffix2, string substring, string line)
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
            suffix2 = suffix1 = null;
            return true;
        }
        suffix1 = split[1];
        if (split.Length == 2)
        {
            suffix2 = null;
            return true;
        }
        if (split.Length == 3)
        {
            suffix2 = split[2];
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
        throw new Exception("Key should not start or end with symbols.");
    }

}