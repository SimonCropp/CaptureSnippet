
using System.Collections.Generic;

namespace CaptureSnippets
{
    public delegate IReadOnlyList<SnippetPackageGroup> ConvertSnippetPackageGroupToList(string key, List<SnippetPackageGroup> packages);
}
