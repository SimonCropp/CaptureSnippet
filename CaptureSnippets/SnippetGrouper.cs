using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static IEnumerable<SnippetGroup> Group(IEnumerable<ReadSnippet> snippets)
        {
            Guard.AgainstNull(snippets, "snippets");
            foreach (var grouping in snippets.GroupBy(x => x.Key))
            {
                var versions = GetVersionGroups(grouping)
                    .OrderByDescending(x => x.Version.MinVersion)
                    .ToList();

                for (var i = 0; i < versions.Count - 1; i++)
                {
                    for (var j = i + 1; j < versions.Count; j++)
                    {
                        // Use list[i] and list[j]
                    }
                }
                yield return new SnippetGroup(key: grouping.Key, versions: versions);
            }
        }

        static IEnumerable<VersionGroup> GetVersionGroups(IEnumerable<ReadSnippet> keyGrouping)
        {
            return keyGrouping.GroupBy(x => x.Version)
                .Select(versionGrouping => new VersionGroup(versionGrouping.Key, GetSnippets(versionGrouping)));
        }

        static IEnumerable<Snippet> GetSnippets(IEnumerable<ReadSnippet> versionGrouping)
        {
            return versionGrouping.Select(x =>
                new Snippet(value: x.Value,
                    endLine: x.EndLine,
                    file: x.File,
                    language: x.Language,
                    startLine: x.StartLine));
        }
    }
}