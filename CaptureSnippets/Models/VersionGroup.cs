using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// Allows <see cref="Snippet"/>s to be grouped by their <see cref="Version"/>.
    /// </summary>
    public class VersionGroup : IEnumerable<Snippet>
    {
        /// <summary>
        ///  The version that all the child <see cref="Snippet"/>s have.
        /// </summary>
        public Version Version;

        /// <summary>
        /// All the snippets with a common <see cref="Version"/>.
        /// </summary>
        public List<Snippet> Snippets = new List<Snippet>();

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