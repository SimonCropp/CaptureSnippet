using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="Snippet"/>s grouped by Key > Version
    /// </summary>
    public class SnippetGroup : IEnumerable<VersionGroup>
    {
        /// <summary>
        /// The key that all child <see cref="VersionGroup"/>s contain.
        /// </summary>
        public string Key;

        /// <summary>
        /// All the <see cref="VersionGroup"/>s with a common key.
        /// </summary>
        public List<VersionGroup> Versions = new List<VersionGroup>();

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
    }
}