using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="IncludeSource"/>s grouped by Key > Package
    /// </summary>
    [DebuggerDisplay("Key={Key}")]
    public class IncludeGroup : IEnumerable<IncludePackageGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetGroup"/>.
        /// </summary>
        public IncludeGroup(string key, IReadOnlyList<IncludePackageGroup> packages)
        {
            Guard.AgainstNull(key, "key");
            Guard.AgainstNull(packages, "packages");
            Key = key;
            Packages = packages;
        }

        /// <summary>
        /// The key that all child <see cref="IncludeGroup"/>s contain.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// All the <see cref="PackageGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<IncludePackageGroup> Packages;

        /// <summary>
        /// Enumerates over <see cref="Packages"/>.
        /// </summary>
        public IEnumerator<IncludePackageGroup> GetEnumerator()
        {
            return Packages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}