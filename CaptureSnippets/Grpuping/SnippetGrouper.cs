using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static SnippetGroups Group(IEnumerable<ReadSnippet> snippets)
        {
            Guard.AgainstNull(snippets, "snippets");

            var groups = new List<SnippetGroup>();
            var errors = new List<string>();
            foreach (var grouping in snippets.GroupBy(x => x.Key))
            {
                string error;
                SnippetGroup snippetGroup;
                if (TryGetGroup(grouping.Key, grouping.ToList(), out snippetGroup, out error))
                {
                    groups.Add(snippetGroup);
                }
                else
                {
                    errors.Add(error);
                }
            }
            return new SnippetGroups(groups, errors);
        }

        static bool TryGetGroup(string key, List<ReadSnippet> readSnippets, out SnippetGroup snippetGroup, out string error)
        {
            var requiredLanguage = readSnippets.Select(x => x.Language).First();
            if (readSnippets.Any(x => x.Language != requiredLanguage))
            {
                error = string.Format("All languages of a give key must be equivalent. Key='{0}'.", key);
                snippetGroup = null;
                return false;
            }

            foreach (var snippet in readSnippets)
            {
                var snippetVersion = snippet.Version;
                var snippets = readSnippets.Where(x => x.Version.Equals(snippetVersion))
                    .ToList();
                if (snippets.Count > 1)
                {
                    var files = string.Join("\r\n", snippets.Select(x => x.FileLocation));
                    error = string.Format("Duplicate key detected. Key='{0}'. Files=\r\n{1}", key, files);
                    snippetGroup = null;
                    return false;
                }
            }

            var containsAllVersionRanges = readSnippets.Any(x=>x.Version.Equals(VersionRange.All));
            var containsNonAllVersionRanges = readSnippets.Any(x=>!x.Version.Equals(VersionRange.All));

            if (containsAllVersionRanges && containsNonAllVersionRanges)
            {
                var files = string.Join("\r\n", readSnippets.Select(x => x.FileLocation));
                error = string.Format("Cannot mix 'all' versions and specific versions. Key='{0}'. Files=\r\n{1}", key, files);
                snippetGroup = null;
                return false;
            }


            var keyGroup = ProcessKeyGroup(readSnippets)
                .OrderByDescending(x => x.Version.VersionForCompare());

            snippetGroup = new SnippetGroup(
                key: key,
                language: requiredLanguage,
                versions: keyGroup);
            error = null;
            return true;
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
                var mergeOccured = false;

                for (var i = 0; i < versions.Count - 1; i++)
                {
                    var left = versions[i];
                    for (var j = i + 1; j < versions.Count; j++)
                    {
                        var right = versions[j];

                        VersionRange newVersion;
                        if (VersionRangeExtensions.CanMerge(left.Range, right.Range, out newVersion))
                        {
                            if (left.ValueHash == right.ValueHash)
                            {
                                left.Range = newVersion;
                                left.Snippets.AddRange(right.Snippets);
                                versions.RemoveAt(j);
                                mergeOccured = true;
                                j--;
                            }
                        }
                    }
                }

                if (!mergeOccured)
                {
                    break;
                }
            }
            return versions.Select(x =>
                new VersionGroup(
                    version: x.Range,
                    value: x.Value,
                    sources: x.Snippets.Select(y =>
                        new SnippetSource(
                            startLine: y.StartLine,
                            endLine: y.EndLine,
                            file: y.File))));
        }

    }
}