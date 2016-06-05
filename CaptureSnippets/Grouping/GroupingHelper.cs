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
        var containsAll = list.Any(x => x.Equals(VersionRange.All));
        var containsNonAll = list.Any(x => !x.Equals(VersionRange.All));
        return containsAll && containsNonAll;
    }

    internal static bool ContainsUndefinedWithNonUndefinedPackage(IEnumerable<Package> enumerable)
    {
        var packages = enumerable.ToList();
        var containsUndefined = packages.Any(x => x == Package.Undefined);
        var containsNonUndefined = packages.Any(x => x != Package.Undefined);
        return containsUndefined && containsNonUndefined;
    }
    internal static bool HasInconsistentComponents(IEnumerable<Component> enumerable)
    {
        return enumerable.Select(x=>x.ValueOrUndefined).Distinct().Count() > 1;
    }
}