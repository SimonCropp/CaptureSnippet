using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    /// <summary>
    /// Allows <see cref="Snippet"/>s to be grouped by their <see cref="Version"/>.
    /// </summary>
    public class VersionGroup : IEnumerable<Snippet>
    {
        /// <summary>
        /// Initialise a new insatnce of <see cref="VersionGroup"/>.
        /// </summary>
        public VersionGroup(Version version, IEnumerable<Snippet> snippets)
        {
            Guard.AgainstNull(version,"version");
            Guard.AgainstNull(snippets, "snippets");
            Version = version;
            Snippets = snippets.ToList();
        }
        /// <summary>
        ///  The version that all the child <see cref="Snippet"/>s have.
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// All the snippets with a common <see cref="Version"/>.
        /// </summary>
        public readonly IEnumerable<Snippet> Snippets;

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
    }
}