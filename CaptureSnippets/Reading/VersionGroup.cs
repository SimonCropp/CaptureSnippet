using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{

    [DebuggerDisplay("Version={Version}, Count={Snippets.Count}}")]
    public class VersionGroup : IEnumerable<Snippet>
    {

        public readonly IReadOnlyList<Snippet> Snippets;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;
        public readonly VersionRange Version;
        public readonly string Directory;
        public readonly bool IsCurrent;
        public readonly string Package;

        public VersionGroup(VersionRange version, string directory, bool isCurrent, string package, IReadOnlyList<Snippet> snippets)
        {
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Guard.AgainstNullAndEmpty(package, nameof(package));
            Snippets = snippets;
            Version = version;
            Directory = directory;
            IsCurrent = isCurrent;
            Package = package;
            Lookup = snippets.ToDictionary();
        }

        public IReadOnlyList<Snippet> this[string key]
        {
            get
            {
                var item = Snippets.Where(snippet => snippet.Key == key).ToList();
                if (!item.Any())
                {
                    throw new Exception($"Could not find snippets for '{key}'");
                }
                return item;
            }
        }

        /// <summary>
        /// Enumerates over <see cref="Snippets"/>.
        /// </summary>
        public IEnumerator<Snippet> GetEnumerator()
        {
            return Snippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $@"VersionGroup.
  Version: {Version}
";
        }
    }
}