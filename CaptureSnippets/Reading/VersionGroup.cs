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

        public VersionGroup(VersionRange version, IReadOnlyList<Snippet> snippets)
        {
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(snippets, nameof(snippets));
            Snippets = snippets;
            Version = version;
        }

        public readonly IReadOnlyList<Snippet> Snippets;

        public readonly VersionRange Version;


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