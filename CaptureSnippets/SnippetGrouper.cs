using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static IEnumerable<SnippetGroup> Group(IEnumerable<ReadSnippet> snippets)
        {
            Guard.AgainstNull(snippets, "snippets");
            foreach (var grouping in snippets.GroupBy(x => x.Key))
            {
                var readSnippets = grouping.ToList();
                var requiredLanguage = readSnippets.Select(x => x.Language).First();
                if (readSnippets.Any(x => x.Language != requiredLanguage))
                {
                    throw new Exception(string.Format("All laguages of a give key must be equivalent. Key='{0}'.", grouping.Key));
                }

                var keyGroup = ProcessKeyGroup(readSnippets)
                    .OrderByDescending(x => x.Version.VersionForCompare());

                yield return new SnippetGroup(
                    key: grouping.Key,
                    language: requiredLanguage,
                    versions:keyGroup);
            }
        }

        class MergedSnippets
        {
            public VersionRange Range;
            public int ValueHash;
            public string Value;
            public List<ReadSnippet> Snippets;
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