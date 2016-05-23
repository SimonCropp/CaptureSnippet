using System.Collections.Generic;

namespace CaptureSnippets
{
    public delegate IReadOnlyList<IncludePackageGroup> ConvertIncludePackageGroupToList(string key, List<IncludePackageGroup> packages);
}