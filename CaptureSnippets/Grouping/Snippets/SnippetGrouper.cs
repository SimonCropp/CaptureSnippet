using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static SnippetGroups Group(IEnumerable<ReadSnippet> readItems, ConvertSnippetPackageGroupToList convertPackageGroupToList = null)
        {
            Guard.AgainstNull(readItems, "readItems");

            var groups = new List<SnippetGroup>();
            var errors = new List<string>();
            foreach (var grouping in readItems.GroupBy(x => x.Key))
            {
                string error;
                SnippetGroup group;
                if (TryGetGroup(grouping.Key, grouping.ToList(), convertPackageGroupToList, out group, out error))
                {
                    groups.Add(group);
                    continue;
                }
                errors.Add(error);
            }
            return new SnippetGroups(groups, errors);
        }

        static bool TryGetPackageGroup(string key, Package package, List<ReadSnippet> readItems, out SnippetPackageGroup packageGroup, out string error)
        {
            packageGroup = null;

            if (GroupingHelper.ContainsDuplicateVersion(readItems.Select(x=>x.Version)))
            {
                var files = string.Join("\r\n", readItems.Select(x => x.FileLocation));
                error = $"Duplicate version detected. Key='{key}'. Package='{package.ValueOrUndefined}'. Files=\r\n{files}";
                return false;
            }

            if (GroupingHelper.ContainsVersionConflictsWithAll(readItems.Select(x => x.Version)))
            {
                var files = string.Join("\r\n", readItems.Select(x => x.FileLocation));
                error = $"Cannot mix 'all' versions and specific versions. Key='{key}'. Files=\r\n{files}";
                return false;
            }

            var keyGroup = ProcessKeyGroup(readItems)
                .OrderByDescending(x => x.Version.VersionForCompare())
                .ToList();
            packageGroup = new SnippetPackageGroup(package, keyGroup);
            error = null;
            return true;
        }

        static bool TryGetGroup(string key, List<ReadSnippet> readItems, ConvertSnippetPackageGroupToList convertPackageGroupToList, out SnippetGroup group, out string error)
        {
            group = null;
            if (LanguagesAreInConsistent(readItems))
            {
                error = $"All languages of a give key must be equivalent. Key='{key}'.";
                return false;
            }
            if (MixesEmptyPackageWithNonEmpty(readItems, out error))
            {
                return false;
            }
            var packageGroups = new List<SnippetPackageGroup>();

            foreach (var package in readItems.GroupBy(x => x.Package.ValueOrUndefined, snippet => snippet))
            {
                SnippetPackageGroup packageGroup;
                var itemsForPackage = package.ToList();
                if (!TryGetPackageGroup(key, itemsForPackage.First().Package, itemsForPackage, out packageGroup, out error))
                {
                    return false;
                }
                packageGroups.Add(packageGroup);
            }

            if (convertPackageGroupToList == null)
            {
                group = new SnippetGroup(
                    key: key,
                    language: readItems.First().Language,
                    packages: packageGroups.OrderBy(_ => _.Package.ValueOrUndefined).ToList());
                return true;
            }
            IReadOnlyList<SnippetPackageGroup> result;
            try
            {
                result = convertPackageGroupToList(key, packageGroups);
            }
            catch (Exception exception)
            {
                error = $"Could not convert PackageGroup to list. Key='{key}'.";
                throw new Exception(error, exception);
            }
            group = new SnippetGroup(
                key: key,
                language: readItems.First().Language,
                packages: result);
            return true;
        }


        static bool MixesEmptyPackageWithNonEmpty(List<ReadSnippet> readItems, out string error)
        {
            if (!GroupingHelper.ContainsUndefinedWithNonUndefinedPackage(readItems.Select(x => x.Package)))
            {
                error = null;
                return false;
            }
            var builder = new StringBuilder($"Mixes empty packages with non empty packages. Key='{readItems.First().Key}'.\r\nItems:\r\n");
            foreach (var item in readItems)
            {
                builder.AppendLine($"   Location: '{item.FileLocation}'. Package: {item.Package.ValueOrUndefined}");
            }
            error = builder.ToString();
            return true;
        }


        static bool LanguagesAreInConsistent(List<ReadSnippet> readItems)
        {
            var requiredLanguage = readItems.First().Language;
            return readItems.Any(x => x.Language != requiredLanguage);
        }

        internal static IEnumerable<SnippetVersionGroup> ProcessKeyGroup(List<ReadSnippet> readItems)
        {
            var versions = readItems.Select(x => new MergedSnippets
            {
                Range = x.Version,
                ValueHash = x.ValueHash,
                Value = x.Value,
                Items = new List<ReadSnippet> {x}
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
                        left.Items.AddRange(right.Items);
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

        static SnippetVersionGroup ConstructVersionGroup(MergedSnippets merged)
        {
            var sources = merged.Items
                .Select(y =>
                    new SnippetSource(
                        version: y.Version,
                        startLine: y.StartLine,
                        endLine: y.EndLine,
                        file: y.Path))
                .ToList();
            return new SnippetVersionGroup(
                version: merged.Range,
                value: merged.Value,
                sources: sources);
        }
    }
}