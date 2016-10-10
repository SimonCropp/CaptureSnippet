using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Package={Identifier, Count={Versions.Count}}")]
    public class Package
    {
        public readonly string Identifier;
        public readonly IReadOnlyList<VersionGroup> Versions;
        public readonly IReadOnlyList<Snippet> Snippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public Package(string identifier, IReadOnlyList<VersionGroup> versions)
        {
            Guard.AgainstNull(versions, nameof(versions));
            Guard.AgainstNullAndEmpty(identifier, nameof(identifier));
            Identifier = identifier;
            Versions = versions;
            Snippets = versions.SelectMany(_ => _.Snippets).Distinct().ToList();
            Lookup = Snippets.ToDictionary();
        }

        public override string ToString()
        {
            return $@"Package.
  Identifier: {Identifier}
";
        }
    }
}