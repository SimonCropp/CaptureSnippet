using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Identifier={Identifier}, Count={Packages.Count}")]
    public class Component
    {
        public string Identifier { get; }
        public IReadOnlyList<Package> Packages { get; }
        public string Directory { get; }
        public IReadOnlyList<Snippet> Snippets { get; }
        public IReadOnlyList<Snippet> Shared { get; }
        public IReadOnlyList<VersionGroup> AllVersions { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }

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