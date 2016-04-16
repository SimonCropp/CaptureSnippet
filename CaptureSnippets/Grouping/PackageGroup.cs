using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="VersionGroup"/>s grouped by Package > Version
    /// </summary>
    [DebuggerDisplay("Package={Package}")]
    public class PackageGroup : IEnumerable<VersionGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="PackageGroup"/>.
        /// </summary>
        public PackageGroup(string package, IReadOnlyList<VersionGroup> versions)
        {
            Guard.AgainstEmpty(package, "package");
            Guard.AgainstNull(versions, "versions");
            Package = package;
            Versions = versions;
        }

        /// <summary>
        /// The key that all child <see cref="VersionGroup"/>s contain.
        /// </summary>
        public readonly string Package;

        /// <summary>
        /// All the <see cref="VersionGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<VersionGroup> Versions;

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