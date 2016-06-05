using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="IncludeSource"/>s grouped by Key > Package
    /// </summary>
    [DebuggerDisplay("Key={Key}, Component={Component}")]
    public class IncludeGroup : IEnumerable<IncludePackageGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetGroup"/>.
        /// </summary>
        public IncludeGroup(string key, Component component, IReadOnlyList<IncludePackageGroup> packages)
        {
            Guard.AgainstNull(key, nameof(key));
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNull(component, nameof(component));
            Key = key;
            Packages = packages;
            Component = component;
        }

        /// <summary>
        /// The key that all child <see cref="IncludeGroup"/>s contain.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The <see cref="Component"/> for this <see cref="IncludeGroup"/>.
        /// </summary>
        public readonly Component Component;

        /// <summary>
        /// All the <see cref="IncludePackageGroup"/>s with a common key.
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