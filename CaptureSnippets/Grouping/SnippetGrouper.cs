using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static SnippetGroups Group(IEnumerable<ReadSnippet> snippets, ConvertPackageGroupToList convertPackageGroupToList = null)
        {
            Guard.AgainstNull(snippets, "snippets");

            var groups = new List<SnippetGroup>();
            var errors = new List<string>();
            foreach (var grouping in snippets.GroupBy(x => x.Key))
            {
                string error;
                SnippetGroup snippetGroup;
                if (TryGetSnippetGroup(grouping.Key, grouping.ToList(), convertPackageGroupToList, out snippetGroup, out error))
                {
                    groups.Add(snippetGroup);
                    continue;
                }
                errors.Add(error);
            }
            return new SnippetGroups(groups, errors);
        }

        static bool TryGetPackageGroup(string key, string package, List<ReadSnippet> readSnippets, out PackageGroup packageGroup, out string error)
        {
            packageGroup = null;

            if (ContainsDuplicateVersion(readSnippets))
            {
                var files = string.Join("\r\n", readSnippets.Select(x => x.FileLocation));
                error = $"Duplicate version detected. Key='{key}'. Package='{package}'. Files=\r\n{files}";
                return false;
            }

            if (ContainsVersionConflictsWithAll(readSnippets))
            {
                var files = string.Join("\r\n", readSnippets.Select(x => x.FileLocation));
                error = $"Cannot mix 'all' versions and specific versions. Key='{key}'. Files=\r\n{files}";
                return false;
            }

            var keyGroup = ProcessKeyGroup(readSnippets)
                .OrderByDescending(x => x.Version.VersionForCompare())
                .ToList();
            packageGroup = new PackageGroup(package, keyGroup);
            error = null;
            return true;
        }

        internal static bool ContainsDuplicateVersion(List<ReadSnippet> readSnippets)
        {
            return readSnippets.Select(snippet => snippet.Version)
                .Select(snippetVersion => readSnippets.Where(x => x.Version.Equals(snippetVersion)))
                .Any(snippets => snippets.Count() > 1);
        }

        static bool ContainsVersionConflictsWithAll(List<ReadSnippet> readSnippets)
        {
            var containsAllVersionRanges = readSnippets.Any(x => x.Version.Equals(VersionRange.All));
            var containsNonAllVersionRanges = readSnippets.Any(x => !x.Version.Equals(VersionRange.All));
            return containsAllVersionRanges && containsNonAllVersionRanges;
        }

        static bool TryGetSnippetGroup(string key, List<ReadSnippet> readSnippets, ConvertPackageGroupToList convertPackageGroupToList, out SnippetGroup snippetGroup, out string error)
        {
            snippetGroup = null;
            error = null;
            if (LanguagesAreInConsistent(readSnippets))
            {
                error = $"All languages of a give key must be equivalent. Key='{key}'.";
                return false;
            }
            if (MixesEmptyPackageWithNonEmpty(readSnippets, out error))
            {
                return false;
            }
            var packageGroups = new List<PackageGroup>();

            foreach (var package in readSnippets.GroupBy(x => x.Package))
            {
                PackageGroup packageGroup;
                if (!TryGetPackageGroup(key, package.Key, package.ToList(), out packageGroup, out error))
                {
                    return false;
                }
                packageGroups.Add(packageGroup);
            }

            if (convertPackageGroupToList == null)
            {
                snippetGroup = new SnippetGroup(
                    key: key,
                    language: readSnippets.First().Language,
                    packages: packageGroups);
                return true;
            }
            var result = convertPackageGroupToList(key, packageGroups);
            if (!result.Success)
            {
                error = $"Could not convert PackageGroup to list. Key='{key}'." + result.ErrorMessage;
                return false;
            }
            snippetGroup = new SnippetGroup(
                key: key,
                language: readSnippets.First().Language,
                packages: result.Value);
            return true;
        }


        static bool MixesEmptyPackageWithNonEmpty(List<ReadSnippet> readSnippets, out string error)
        {
            var containsNullPackages = readSnippets.Any(x => x.Package == null);
            var containsNonNullPackages = readSnippets.Any(x => x.Package != null);
            if (containsNullPackages && containsNonNullPackages)
            {
                var builder = new StringBuilder($"Mixes empty packages with non empty packages. Key='{readSnippets.First().Key}'.\r\nSnippets:\r\n");
                foreach (var snippet in readSnippets)
                {
                    builder.AppendLine($"   Location: '{snippet.FileLocation}'. Package: {snippet.Package}");
                }
                error = builder.ToString();
                return true;
            }
            error = null;
            return false;
        }

        static bool LanguagesAreInConsistent(List<ReadSnippet> readSnippets)
        {
            var requiredLanguage = readSnippets.First().Language;
            return readSnippets.Any(x => x.Language != requiredLanguage);
        }

        internal static IEnumerable<VersionGroup> ProcessKeyGroup(List<ReadSnippet> readSnippets)
        {
            var versions = readSnippets.Select(x => new MergedSnippets
            {
                Range = x.Version,
                ValueHash = x.ValueHash,
                Value = x.Value,
                Snippets = new List<ReadSnippet> {x}
            }).ToList();

            while (true)
            {
                var mergeOccurred = false;

                for (var i = 0; i < versions.Count - 1; i++)
                {
                    var left = versions[i];
                    for (var j = i + 1; j < versions.Count; j++)
                    {
                        var right = versions[j];

                        VersionRange newVersion;
                        if (!VersionRangeExtensions.CanMerge(left.Range, right.Range, out newVersion))
                        {
                            continue;
                        }
                        if (left.ValueHash != right.ValueHash)
                        {
                            continue;
                        }
                        left.Range = newVersion;
                        left.Snippets.AddRange(right.Snippets);
                        versions.RemoveAt(j);
                        mergeOccurred = true;
                        j--;
                    }
                }

                if (!mergeOccurred)
                {
                    break;
                }
            }
            return versions.Select(ConstructVersionGroup);
        }

        static VersionGroup ConstructVersionGroup(MergedSnippets mergedSnippets)
        {
            var snippetSources = mergedSnippets.Snippets
                .Select(y =>
                    new SnippetSource(
                        version: y.Version,
                        startLine: y.StartLine,
                        endLine: y.EndLine,
                        file: y.Path))
                .ToList();
            return new VersionGroup(
                version: mergedSnippets.Range,
                value: mergedSnippets.Value,
                sources: snippetSources);
        }
    }
}