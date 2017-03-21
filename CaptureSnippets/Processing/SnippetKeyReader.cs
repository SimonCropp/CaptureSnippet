using System;

static class SnippetKeyReader
{
    public static bool TryExtractKeyFromLine(string line, out string key)
    {
        if (!line.StartsWith("snippet:"))
        {
            key = null;
            return false;
        }

        key = line.Substring(8);

        if (!key.StartsWith(" "))
        {
            throw new Exception($"Invalid syntax for the snippet '{key}': There must be a space before the start of the key.");
        }

        key = key.Trim();
        return true;
    }
}