using System.Collections.Generic;

namespace CaptureSnippets
{
    public delegate Result<IReadOnlyList<PackageGroup>> ConvertPackageGroupToList(string key, List<PackageGroup> packages);
}