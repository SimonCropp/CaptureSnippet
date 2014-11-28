using System.Collections.Generic;

namespace CaptureSnippets
{
    public class ProcessResult
    {
        public string Text;

        public List<string> UsedSnippets = new List<string>();
        public List<MissingSnippet> MissingSnippet = new List<MissingSnippet>();

    }

    public class MissingSnippet
    {
        public string Key;
        public int Line;
    }
}