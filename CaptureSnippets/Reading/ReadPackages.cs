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
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly IReadOnlyList<Snippet> AllSnippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public ReadPackages(IReadOnlyList<Package> packages, IReadOnlyList<Snippet> shared)
        {
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNull(shared, nameof(shared));
            Packages = packages;
            Shared = shared;
            AllSnippets = packages.SelectMany(_ => _.AllSnippets).ToList();
            SnippetsInError = AllSnippets.Where(_ => _.IsInError).ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        public readonly IReadOnlyList<Snippet> SnippetsInError;

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