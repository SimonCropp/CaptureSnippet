using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Allows <see cref="SnippetSource"/>s to be grouped by their <see cref="VersionRange"/>.
    /// </summary>
    [DebuggerDisplay("Version={Version}, Value={Value}")]
    public class VersionGroup : IEnumerable<SnippetSource>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="VersionGroup"/>.
        /// </summary>
        public VersionGroup(VersionRange version, string value, IReadOnlyList<SnippetSource> sources)
        {
            Guard.AgainstNull(version, nameof(version));
            Guard.AgainstNull(sources, nameof(sources));
            Value = value;
            Version = version;
            Sources = sources;
        }

        /// <summary>
        ///  The version that all the child <see cref="SnippetSource"/>s have.
        /// </summary>
        public readonly VersionRange Version;

        /// <summary>
        /// The contents of the snippet
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// All the snippets with a common <see cref="VersionRange"/>.
        /// </summary>
        public readonly IReadOnlyList<SnippetSource> Sources;

        /// <summary>
        /// Enumerates over <see cref="Sources"/>.
        /// </summary>
        public IEnumerator<SnippetSource> GetEnumerator()
        {
            return Sources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}