using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeParser
    {

        public static bool TryParseVersion(string stringVersion, out VersionRange parsedVersion)
        {
            if (stringVersion == "allversions")
            {
                parsedVersion = VersionRange.All;
                return true;
            }
            int intversion;
            if (int.TryParse(stringVersion, out intversion))
            {
                var semanticVersion = new SemanticVersion(intversion, 0, 0);
                parsedVersion = new VersionRange(semanticVersion);
                return true;
            }
            return VersionRange.TryParse(stringVersion, out parsedVersion);
        }
    }
}