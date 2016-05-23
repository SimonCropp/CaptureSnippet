using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// A snippet after it has been grouped by <see cref="IncludeGrouper"/>.
    /// </summary>
    /// <remarks>Note that <see cref="ReadInclude.Version"/> and <see cref="ReadInclude.Key"/> are not included since they can be infered by the grouping structure.</remarks>
    [DebuggerDisplay("File={File}, Version={Version}")]
    public class IncludeSource
    {
        /// <summary>
        /// Initialise a new instance of <see cref="IncludeSource"/>.
        /// </summary>
        public IncludeSource(string file, VersionRange version)
        {
            Guard.AgainstNull(version, "version");
            File = file;
            Version = version;
        }

        /// <summary>
        /// The <see cref="VersionRange"/> for the snippet.
        /// </summary>
        public readonly VersionRange Version;
        /// <summary>
        /// The file path the snippet was read from.
        /// </summary>
        public readonly string File;
    }
}