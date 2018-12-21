using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Package={Identifier, Count={Versions.Count}}")]
    public class Package
    {
        public string Identifier { get; }
        public IReadOnlyList<VersionGroup> Versions { get; }
        public IReadOnlyList<Snippet> Snippets { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }

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