using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package}")]
    public class SnippetMetaData
    {
        public readonly VersionRange Version;
        public readonly Package Package;

        public SnippetMetaData(VersionRange version, Package package)
        {
            Guard.AgainstNull(package, "package");
            Guard.AgainstNull(version, "version");
            Version = version;
            Package = package;
        }
    }
}