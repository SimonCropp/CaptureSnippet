using System.Collections.Generic;

namespace CaptureSnippets
{
    public delegate IReadOnlyList<PackageGroup> ConvertPackageGroupToList(string key, List<PackageGroup> packages);
}