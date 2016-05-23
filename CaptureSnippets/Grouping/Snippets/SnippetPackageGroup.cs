using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="SnippetVersionGroup"/>s grouped by Package > Version
    /// </summary>
    [DebuggerDisplay("Package={Package.ValueOrNone}")]
    public class SnippetPackageGroup : IEnumerable<SnippetVersionGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetPackageGroup"/>.
        /// </summary>
        public SnippetPackageGroup(Package package, IReadOnlyList<SnippetVersionGroup> versions)
        {
            Guard.AgainstNull(package, "package");
            Guard.AgainstNull(versions, "versions");
            Package = package;
            Versions = versions;
        }

        /// <summary>
        /// The key that all child <see cref="SnippetVersionGroup"/>s contain.
        /// </summary>
        public readonly Package Package;

        /// <summary>
        /// All the <see cref="SnippetVersionGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<SnippetVersionGroup> Versions;

        /// <summary>
        /// Enumerates over <see cref="Versions"/>.
        /// </summary>
        public IEnumerator<SnippetVersionGroup> GetEnumerator()
        {
            return Versions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}