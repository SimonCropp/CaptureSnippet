using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Packages.Count}")]
    public class ReadPackages
    {
        public readonly IReadOnlyList<Package> Packages;
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> Snippets;
        public readonly IReadOnlyList<VersionGroup> Versions;
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;
        public readonly IReadOnlyList<Snippet> SnippetsInError;

        public ReadPackages(IReadOnlyList<Package> packages, string directory, IReadOnlyList<Snippet> shared)
        {
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Guard.AgainstNull(shared, nameof(shared));
            Shared = shared;
            Packages = packages;
            Directory = directory;
            Snippets = packages.SelectMany(_ => _.Snippets).Concat(shared).Distinct().ToList();
            Versions = Packages.SelectMany(_ => _.Versions).Distinct().ToList();
            SnippetsInError = Snippets.Where(_ => _.IsInError).Distinct().ToList();
            Lookup = Snippets.ToDictionary();
        }

        public Package GetPackage(string key)
        {
            var item = Packages.SingleOrDefault(package => package.Identifier == key);
            if (item == null)
            {
                throw new Exception($"Could not find Package for '{key}'");
            }
            return item;
        }
    }
}