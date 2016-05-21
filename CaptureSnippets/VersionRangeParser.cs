using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeParser
    {

        public static bool TryParseVersion(string stringVersion, out VersionRange parsedVersion)
        {
            if (stringVersion == "All")
            {
                parsedVersion = VersionRange.All;
                return true;
            }
            if (stringVersion == "AllFloating")
            {
                parsedVersion = VersionRange.AllFloating;
                return true;
            }
            if (stringVersion == "AllStable")
            {
                parsedVersion = VersionRange.AllStable;
                return true;
            }
            if (stringVersion == "AllStableFloating")
            {
                parsedVersion = VersionRange.AllStableFloating;
                return true;
            }
            if (stringVersion == "None")
            {
                parsedVersion = VersionRange.None;
                return true;
            }
            int majorPart;
            if (int.TryParse(stringVersion, out majorPart))
            {
                parsedVersion = new VersionRange(
                    minVersion: new NuGetVersion(majorPart, 0, 0),
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(majorPart + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            NuGetVersion minVersion;
            if (NuGetVersion.TryParse(stringVersion, out minVersion))
            {
                parsedVersion = new VersionRange(
                    minVersion: minVersion,
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(minVersion.Major + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            return VersionRange.TryParse(stringVersion, out parsedVersion);
        }
    }
}