using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Version={Version}, Package={Package}")]
    public class SnippetMetaData
    {
        public readonly VersionRange Version;
        public readonly string Package;

        public SnippetMetaData(VersionRange version, string package)
        {
            Guard.AgainstNull(version, "version");
            Version = version;
            Package = package;
        }
    }
}