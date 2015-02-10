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
                var version = snippetGroup.FirstOrDefault(x => x.Version == readSnippet.Version);
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
                                  EndLine = readSnippet.EndLine,
                                  File = readSnippet.File,
                                  Language = readSnippet.Language,
                                  StartLine = readSnippet.StartLine,
                              };
                version.Snippets.Add(snippet);
            }

            foreach (var snippetGroup in snippetGroups)
            {
                snippetGroup.Versions = snippetGroup.OrderByDescending(x => x.Version).ToList();
                foreach (var versionGroup in snippetGroup)
                {
                    versionGroup.Snippets = versionGroup.OrderBy(x => x.Language).ToList();
                }
            }
            return snippetGroups;
        }
    }
}