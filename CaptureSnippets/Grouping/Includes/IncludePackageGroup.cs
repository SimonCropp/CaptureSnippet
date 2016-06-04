using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="IncludeVersionGroup"/>s grouped by Package > Version
    /// </summary>
    [DebuggerDisplay("Package={Package.ValueOrUndefined}")]
    public class IncludePackageGroup : IEnumerable<IncludeVersionGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="PackageGroup"/>.
        /// </summary>
        public IncludePackageGroup(Package package, IReadOnlyList<IncludeVersionGroup> versions)
        {
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(versions, nameof(versions));
            Package = package;
            Versions = versions;
        }

        /// <summary>
        /// The key that all child <see cref="IncludeVersionGroup"/>s contain.
        /// </summary>
        public readonly Package Package;

        /// <summary>
        /// All the <see cref="IncludeVersionGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<IncludeVersionGroup> Versions;

        /// <summary>
        /// Enumerates over <see cref="Versions"/>.
        /// </summary>
        public IEnumerator<IncludeVersionGroup> GetEnumerator()
        {
            return Versions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}