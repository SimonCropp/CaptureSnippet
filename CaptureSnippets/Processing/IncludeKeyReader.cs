namespace CaptureSnippets
{
    static class IncludeKeyReader
    {
        public static bool TryExtractKeyFromLine(string line, out string key)
        {
            if (line.StartsWith("include:"))
            {
                key = line.Substring(8).Trim();
                return true;
            }
            key = null;
            return false;
        }
    }
}