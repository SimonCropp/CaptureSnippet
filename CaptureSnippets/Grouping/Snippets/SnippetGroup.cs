using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CaptureSnippets
{
    /// <summary>
    /// A hierarchy of <see cref="SnippetSource"/>s grouped by Key > Package
    /// </summary>
    [DebuggerDisplay("Key={Key}, Language={Language}")]
    public class SnippetGroup : IEnumerable<SnippetPackageGroup>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SnippetGroup"/>.
        /// </summary>
        public SnippetGroup(string key, string language, IReadOnlyList<SnippetPackageGroup> packages)
        {
            Guard.AgainstNull(key, nameof(key));
            Guard.AgainstNull(packages, nameof(packages));
            Guard.AgainstNull(language, nameof(language));
            Guard.AgainstUpperCase(language, nameof(language));
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
        /// All the <see cref="SnippetPackageGroup"/>s with a common key.
        /// </summary>
        public readonly IReadOnlyList<SnippetPackageGroup> Packages;

        /// <summary>
        /// Enumerates over <see cref="Packages"/>.
        /// </summary>
        public IEnumerator<SnippetPackageGroup> GetEnumerator()
        {
            return Packages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}