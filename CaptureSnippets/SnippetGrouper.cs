using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static IEnumerable<SnippetGroup> Group(IEnumerable<ReadSnippet> snippets)
        {
            return snippets.GroupBy(x => x.Key)
                .Select(grouping =>
                    new SnippetGroup(
                        key: grouping.Key,
                        versions: GetVersionGroups(grouping).ToList()));
        }

        static IEnumerable<VersionGroup> GetVersionGroups(IGrouping<string, ReadSnippet> keyGrouping)
        {
            return keyGrouping.GroupBy(x => x.Version, VersionEquator.Instance)
                .Select(versionGrouping => new VersionGroup(versionGrouping.Key, GetSnippets(versionGrouping)));
        }

        static IEnumerable<Snippet> GetSnippets(IGrouping<Version, ReadSnippet> versionGrouping)
        {
            foreach (var readSnippet in versionGrouping)
            {
                yield return new Snippet
                {
                    Value = readSnippet.Value,
                    EndLine = readSnippet.EndLine,
                    File = readSnippet.File,
                    Language = readSnippet.Language,
                    StartLine = readSnippet.StartLine,
                };
            }
        }
    }
}