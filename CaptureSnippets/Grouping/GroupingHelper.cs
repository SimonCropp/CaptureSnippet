using System.Collections.Generic;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;

class GroupingHelper
{
    internal static bool ContainsDuplicateVersion(IEnumerable<VersionRange> versionRanges)
    {
        var list = versionRanges.ToList();
        return list.Select(snippetVersion => list.Where(x => x.Equals(snippetVersion)))
            .Any(snippets => snippets.Count() > 1);
    }

    internal static bool ContainsVersionConflictsWithAll(IEnumerable<VersionRange> versionRanges)
    {
        var list = versionRanges.ToList();
        var containsAllVersionRanges = list.Any(x => x.Equals(VersionRange.All));
        var containsNonAllVersionRanges = list.Any(x => !x.Equals(VersionRange.All));
        return containsAllVersionRanges && containsNonAllVersionRanges;
    }

    internal static bool ContainsUndefinedWithNonUndefinedPackage(IEnumerable<Package> enumerable)
    {
        var packages = enumerable.ToList();
        var containsUndefinedPackages = packages.Any(x => x == Package.Undefined);
        var containsNonUndefinedPackages = packages.Any(x => x != Package.Undefined);
        return containsUndefinedPackages && containsNonUndefinedPackages;
    }
}