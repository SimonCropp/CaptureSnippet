using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class IncludeGrouper
    {
        public static IncludeGroups Group(IEnumerable<ReadInclude> readItems, GetPackageOrderForComponent packageOrder = null)
        {
            Guard.AgainstNull(readItems, nameof(readItems));

            var groups = new List<IncludeGroup>();
            var errors = new List<string>();
            foreach (var grouping in readItems.GroupBy(x => x.Key))
            {
                string error;
                IncludeGroup group;
                if (TryGetGroup(grouping.Key, grouping.ToList(), packageOrder, out group, out error))
                {
                    groups.Add(group);
                    continue;
                }
                errors.Add(error);
            }
            return new IncludeGroups(groups, errors);
        }

        static bool TryGetPackageGroup(string key, Package package, List<ReadInclude> readItems, out IncludePackageGroup packageGroup, out string error)
        {
            packageGroup = null;

            if (GroupingHelper.ContainsDuplicateVersion(readItems.Select(x => x.Version)))
            {
                var files = string.Join("\r\n", readItems.Select(x => x.Path));
                error = $"Duplicate version detected. Key='{key}'. Package='{package.ValueOrUndefined}'. Files=\r\n{files}";
                return false;
            }

            if (GroupingHelper.ContainsVersionConflictsWithAll(readItems.Select(x => x.Version)))
            {
                var files = string.Join("\r\n", readItems.Select(x => x.Path));
                error = $"Cannot mix 'all' versions and specific versions. Key='{key}'. Files=\r\n{files}";
                return false;
            }

            var keyGroup = ProcessKeyGroup(readItems)
                .OrderByDescending(x => x.Version.VersionForCompare())
                .ToList();
            packageGroup = new IncludePackageGroup(package, keyGroup);
            error = null;
            return true;
        }

        static bool TryGetGroup(string key, List<ReadInclude> readItems, GetPackageOrderForComponent packageOrder, out IncludeGroup group, out string error)
        {
            group = null;
            if (MixesEmptyPackageWithNonEmpty(readItems, out error))
            {
                return false;
            }
            if (HasInconsistentComponents(readItems, out error))
            {
                return false;
            }
            var packageGroups = new List<IncludePackageGroup>();

            foreach (var package in readItems.GroupBy(x => x.Package.ValueOrUndefined, snippet => snippet))
            {
                IncludePackageGroup packageGroup;
                var itemsForPackage = package.ToList();
                if (!TryGetPackageGroup(key, itemsForPackage.First().Package, itemsForPackage, out packageGroup, out error))
                {
                    return false;
                }
                packageGroups.Add(packageGroup);
            }

            var first = readItems.First();
            var packages = GetOrderedPackages(key, packageOrder, first.Component, packageGroups).ToList();
            group = new IncludeGroup(
                key: key,
                packages: packages,
                component: first.Component);
            return true;
        }

        static IEnumerable<IncludePackageGroup> GetOrderedPackages(string key, GetPackageOrderForComponent packageOrder, Component component, List<IncludePackageGroup> packageGroups)
        {
            if (packageOrder == null || component == Component.Undefined)
            {
                return packageGroups.OrderBy(_ => _.Package.ValueOrUndefined);
            }
            List<string> result;
            try
            {
                result = packageOrder(component).ToList();
            }
            catch (Exception exception)
            {
                var errorMessage = $"Error getting package order. Key='{key}', Component='{component}'.";
                throw new Exception(errorMessage, exception);
            }

            return packageGroups.OrderBy(_ =>
            {
                try
                {
                    return result.IndexOf(_.Package.ValueOrUndefined);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Error getting package index. Key='{key}', Component='{component}', Package='{_.Package.ValueOrUndefined}'.";
                    throw new Exception(errorMessage, exception);
                }
            });
        }
        static bool HasInconsistentComponents(List<ReadInclude> readItems, out string error)
        {
            if (!GroupingHelper.HasInconsistentComponents(readItems.Select(x => x.Component)))
            {
                error = null;
                return false;
            }
            var builder = new StringBuilder($"Has inconsistent components. Key='{readItems.First().Key}'.\r\nItems:\r\n");
            foreach (var item in readItems)
            {
                builder.AppendLine($"   Location: '{item.Path}'. Component: {item.Component.ValueOrUndefined}");
            }
            error = builder.ToString();
            return true;
        }

        static bool MixesEmptyPackageWithNonEmpty(List<ReadInclude> readItems, out string error)
        {
            if (!GroupingHelper.ContainsUndefinedWithNonUndefinedPackage(readItems.Select(x => x.Package)))
            {
                error = null;
                return false;
            }
            var builder = new StringBuilder($"Mixes empty packages with non empty packages. Key='{readItems.First().Key}'.\r\nItems:\r\n");
            foreach (var item in readItems)
            {
                builder.AppendLine($"   Location: '{item.Path}'. Package: {item.Package.ValueOrUndefined}");
            }
            error = builder.ToString();
            return true;
        }

        static IEnumerable<IncludeVersionGroup> ProcessKeyGroup(List<ReadInclude> readItems)
        {
            var versions = readItems.Select(x => new MergedIncludes
            {
                Range = x.Version,
                ValueHash = x.ValueHash,
                Value = x.Value,
                Items = new List<ReadInclude> {x}
            })
                .ToList();

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

        static IncludeVersionGroup ConstructVersionGroup(MergedIncludes merged)
        {
            var sources = merged.Items
                .Select(y =>
                    new IncludeSource(
                        version: y.Version,
                        file: y.Path))
                .ToList();
            return new IncludeVersionGroup(
                version: merged.Range,
                value: merged.Value,
                sources: sources);
        }
    }
}