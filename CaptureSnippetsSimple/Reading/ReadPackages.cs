using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Packages.Count}")]
    public class ReadPackages
    {
        public IReadOnlyList<Package> Packages { get; }
        public string Directory { get; }
        public IReadOnlyList<Snippet> Snippets { get; }
        public IReadOnlyList<VersionGroup> Versions { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }
        public IReadOnlyList<Snippet> SnippetsInError { get; }

        public ReadPackages(IReadOnlyList<Package> packages, string directory)
        {
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Packages = packages;
            Directory = directory;
            Snippets = packages.SelectMany(_ => _.Snippets).Distinct().ToList();
            Versions = Packages.SelectMany(_ => _.Versions).Distinct().ToList();
            SnippetsInError = Snippets.Where(_ => _.IsInError).Distinct().ToList();
            Lookup = Snippets.ToDictionary();
        }
    }
}