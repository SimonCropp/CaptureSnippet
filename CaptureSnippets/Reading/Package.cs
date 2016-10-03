using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Package={Identifier, Count={Versions.Count}}")]
    public class Package : IEnumerable<VersionGroup>
    {
        public readonly string Identifier;
        public readonly IReadOnlyList<VersionGroup> Versions;
        public readonly IReadOnlyList<Snippet> AllSnippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public Package(string identifier, IReadOnlyList<VersionGroup> versions)
        {
            Guard.AgainstNull(versions, nameof(versions));
            Guard.AgainstNullAndEmpty(identifier, nameof(identifier));
            Identifier = identifier;
            Versions = versions;
            AllSnippets = versions.SelectMany(_ => _.Snippets).ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        /// <summary>
        /// Enumerates over <see cref="Versions"/>.
        /// </summary>
        public IEnumerator<VersionGroup> GetEnumerator()
        {
            return Versions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $@"Package.
  Identifier: {Identifier}
";
        }
    }
}