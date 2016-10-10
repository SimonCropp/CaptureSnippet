using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Identifier={Identifier}, Count={Packages.Count}")]
    public class Component
    {
        public readonly string Identifier;
        public readonly IReadOnlyList<Package> Packages;
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> Snippets;
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly IReadOnlyList<VersionGroup> AllVersions;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public Component(string identifier, IReadOnlyList<Package> packages, string directory, IReadOnlyList<Snippet> shared)
        {
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNullAndEmpty(identifier, nameof(identifier));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Guard.AgainstNull(shared, nameof(shared));
            Identifier = identifier;
            Directory = directory;
            Shared = shared;
            Packages = packages;
            Snippets = Packages.SelectMany(_ => _.Snippets).Concat(shared).Distinct().ToList();
            AllVersions = Packages.SelectMany(_ => _.Versions).Distinct().ToList();
            Lookup = Snippets.ToDictionary();
        }

        public override string ToString()
        {
            return $@"Component.
  Identifier: {Identifier}
";
        }
    }
}