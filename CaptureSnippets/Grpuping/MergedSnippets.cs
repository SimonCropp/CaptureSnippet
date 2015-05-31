using System.Collections.Generic;
using NuGet.Versioning;

namespace CaptureSnippets
{
    class MergedSnippets
    {
        public VersionRange Range;
        public int ValueHash;
        public string Value;
        public List<ReadSnippet> Snippets;
    }
}