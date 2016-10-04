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
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> AllSnippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public Component(string identifier, IReadOnlyList<Package> packages, IReadOnlyList<Snippet> shared, string directory)
        {
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNullAndEmpty(identifier, nameof(identifier));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Identifier = identifier;
            //todo: add shared to errors
            Shared = shared;
            Directory = directory;
            Packages = packages;
            AllSnippets = GetAll().ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        IEnumerable<Snippet> GetAll()
        {
            foreach (var package in Packages)
            {
                foreach (var snippet in package.AllSnippets)
                {
                    yield return snippet;
                }
            }
            foreach (var snippet in Shared)
            {
                yield return snippet;
            }
        }

        public override string ToString()
        {
            return $@"Component.
  Identifier: {Identifier}
";
        }
    }
}