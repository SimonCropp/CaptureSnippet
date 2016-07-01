using System;
using System.Collections.Generic;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Range={Range}, Value={Value}")]
    class MergedSnippets
    {
        public VersionRange Range;
        public int ValueHash;
        public string Value;
        public List<ReadSnippet> Items;

        public override string ToString()
        {
            return $@"MergedSnippets. 
  Range: {Range}
  Value: {Value}
";
        }
    }
}