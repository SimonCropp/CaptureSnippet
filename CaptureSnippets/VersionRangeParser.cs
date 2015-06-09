using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeParser
    {

        public static bool TryParseVersion(string stringVersion, out VersionRange parsedVersion)
        {
            if (stringVersion == "all")
            {
                parsedVersion = VersionRange.All;
                return true;
            }
            int intversion;
            if (int.TryParse(stringVersion, out intversion))
            {
                var semanticVersion = new SemanticVersion(intversion, 0, 0);
                parsedVersion = new VersionRange(
                    minVersion: semanticVersion,
                    includeMinVersion: true,
                    maxVersion: new SemanticVersion(intversion + 1, 0, 0),
                    includeMaxVersion: false
                    );
                return true;
            }
            return VersionRange.TryParse(stringVersion, out parsedVersion);
        }
    }
}