namespace CaptureSnippets
{
    static class SnippetKeyReader
    {
        public static bool TryExtractKeyFromLine(string line, out string key)
        {
            if (line.StartsWith("snippet:"))
            {
                key = line.Substring(8).Trim();
                return true;
            }
            key = null;
            return false;
        }
    }
}