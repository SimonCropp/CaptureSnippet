using System;

static class CSharpUsingExtractor
{
    const string Pattern = "using ";

    public static Func<string, string> Extract = line =>
    {
        var trimmedLine = line.Trim();
        var usingIndex = trimmedLine.IndexOf(Pattern, StringComparison.InvariantCultureIgnoreCase);

        if (!trimmedLine.StartsWith("//") && usingIndex > -1 && trimmedLine.EndsWith(";", StringComparison.Ordinal))
        {
            string result;

            var ns = trimmedLine.Substring(usingIndex + Pattern.Length);
            var aliasIndex = ns.IndexOf("=", StringComparison.Ordinal);

            if (aliasIndex > -1)
            {
                result = ns.Substring(aliasIndex + 1).Trim();
            }
            else
            {
                result = ns;
            }

            return result.Substring(0, result.Length - 1); //skip the ending semicolon
        }

        return null;
    };
}