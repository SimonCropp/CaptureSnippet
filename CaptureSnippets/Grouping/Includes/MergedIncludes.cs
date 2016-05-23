using System.Collections.Generic;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Range={Range}, Value={Value}")]
    class MergedIncludes
    {
        public VersionRange Range;
        public int ValueHash;
        public string Value;
        public List<ReadInclude> Items;
    }
}