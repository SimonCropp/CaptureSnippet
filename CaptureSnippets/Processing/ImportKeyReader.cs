namespace CaptureSnippets
{
    static class ImportKeyReader
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

        public static bool TryExtractKeyFromLineLegacy(string line, out string key)
        {
            if (TryExtractKeyFromLine(line, out key))
            {
                return true;
            }
            line = line.Replace("  ", " ");
            var indexOfImport = line.IndexOf("<!-- import ");
            var charsToTrim = 12;
            if (indexOfImport == -1)
            {
                charsToTrim = 11;
                indexOfImport = line.IndexOf("<!--import ");
                if (indexOfImport == -1)
                {
                    key = null;
                    return false;
                }
            }
            var substring = line.Substring(indexOfImport + charsToTrim);
            var indexOfFinish = substring.IndexOf("-->");
            if (indexOfFinish == -1)
            {
                key = null;
                return false;
            }
            key = substring.Substring(0, indexOfFinish)
                .TrimNonCharacters();
            return true;
        }

    }
}