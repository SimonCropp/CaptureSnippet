using NuGet.Versioning;

namespace CaptureSnippets
{
    public class SnippetMetaData
    {
        public readonly VersionRange VersionRange;
        public readonly string Package;

        public SnippetMetaData(VersionRange versionRange, string package)
        {
            Guard.AgainstNull(versionRange, "versionRange");
            VersionRange = versionRange;
            Package = package;
        }
    }
}