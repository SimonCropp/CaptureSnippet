using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeParser
    {

        public static bool TryParseVersion(string stringVersion, out VersionRange parsedVersion, string pretext = null)
        {
            Guard.AgainstEmpty(pretext, nameof(pretext));
            if (pretext == null)
            {
                return TryParseStable(stringVersion, out parsedVersion);
            }

            return TryParseUnstable(stringVersion, out parsedVersion, pretext);
        }

        static bool TryParseUnstable(string stringVersion, out VersionRange parsedVersion, string pretext)
        {
            int majorPart;
            if (int.TryParse(stringVersion, out majorPart))
            {
                var releaseLabels = new[]
                {
                    pretext
                };
                parsedVersion = new VersionRange(
                    minVersion: new NuGetVersion(majorPart, 0, 0, releaseLabels, null),
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(majorPart + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            NuGetVersion minVersion;
            var valueWithPre = $"{stringVersion}-{pretext}";
            if (NuGetVersion.TryParse(valueWithPre, out minVersion))
            {
                parsedVersion = new VersionRange(
                    minVersion: minVersion,
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(minVersion.Major + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            var message = $"Could not use prerelease.txt to parse a SemanticVersion. Value attempted:'{valueWithPre}'.";
            throw new Exception(message);
        }

        static bool TryParseStable(string stringVersion, out VersionRange parsedVersion)
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