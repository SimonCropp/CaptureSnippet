using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public static class SnippetGrouper
    {
        public static IEnumerable<SnippetGroup> Group(IEnumerable<ReadSnippet> snippets)
        {
            var snippetGroups = new List<SnippetGroup>();
            foreach (var readSnippet in snippets)
            {
                var snippetGroup = snippetGroups.FirstOrDefault(x => x.Key == readSnippet.Key);
                if (snippetGroup == null)
                {
                    snippetGroup = new SnippetGroup
                              {
                                  Key = readSnippet.Key,
                              };
                    snippetGroups.Add(snippetGroup);
                }
                var version = snippetGroup.Versions.FirstOrDefault(x => x.Version == readSnippet.Version);
                if (version == null)
                {
                    version = new VersionGroup
                               {
                                   Version = readSnippet.Version,
                               };
                    snippetGroup.Versions.Add(version);
                }
                var snippet = new Snippet
                              {
                                  Value = readSnippet.Value,
                                  Language = readSnippet.Language,
                                  Source = readSnippet
                              };
                version.Snippets.Add(snippet);
            }

            foreach (var snippetGroup in snippetGroups)
            {
                snippetGroup.Versions = snippetGroup.Versions.OrderByDescending(x => x.Version).ToList();
                foreach (var versionGroup in snippetGroup.Versions)
                {
                    versionGroup.Snippets = versionGroup.Snippets.OrderBy(x => x.Language).ToList();
                }
            }
            return snippetGroups;
        }
    }
}