using System.Collections;
using System.Collections.Generic;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="SnippetSource"/>s grouped by Key > Package
    /// </summary>
    public class SnippetGroup : IEnumerable<PackageGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetGroup"/>.
        /// </summary>
        public SnippetGroup(string key, string language, IReadOnlyList<PackageGroup> packages)
        {
            Guard.AgainstNull(key, "key");
            Guard.AgainstNull(packages, "packages");
            Guard.AgainstNull(language, "language");
            Guard.AgainstUpperCase(language, "language");
            Key = key;
            Packages = packages;
            Language = language;
        }

        /// <summary>
        /// The key that all child <see cref="SnippetGroup"/>s contain.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// All the <see cref="PackageGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<PackageGroup> Packages;

        /// <summary>
        /// Enumerates over <see cref="Packages"/>.
        /// </summary>
        public IEnumerator<PackageGroup> GetEnumerator()
        {
            return Packages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}