using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Allows <see cref="IncludeSource"/>s to be grouped by their <see cref="VersionRange"/>.
    /// </summary>
    [DebuggerDisplay("Version={Version}, Value={Value}")]
    public class IncludeVersionGroup : IEnumerable<IncludeSource>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="VersionGroup"/>.
        /// </summary>
        public IncludeVersionGroup(VersionRange version, string value, IReadOnlyList<IncludeSource> sources)
        {
            Guard.AgainstNull(version,"version");
            Guard.AgainstNull(sources, "sources");
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
        /// All the includes with a common <see cref="VersionRange"/>.
        /// </summary>
        public readonly IReadOnlyList<IncludeSource> Sources;

        /// <summary>
        /// Enumerates over <see cref="Sources"/>.
        /// </summary>
        public IEnumerator<IncludeSource> GetEnumerator()
        {
            return Sources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}