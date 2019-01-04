using System;

static class StartEndCommentTester
{
    public static bool IsEndCode(string line)
    {
        return line == "<!-- endsnippet -->";
    }

    public static bool IsStart(string line, out string key)
    {
        if (!line.StartsWith("<!-- snippet: ", StringComparison.Ordinal))
        {
            key = null;
            return false;
        }

        var substring = line.Substring(13);

        if (!substring.EndsWith(" -->"))
        {
            throw new Exception($"Expected line to end with ' -->'. Line: '{line}'.");
        }

        key = substring.Substring(0, substring.Length - 3).Trim();
        if (key.Length == 0)
        {
            throw new Exception($"No Key could be derived. Line: '{line}'.");
        }

        KeyValidator.ValidateKeyDoesNotStartOrEndWithSymbol(key);
        return true;
    }
}