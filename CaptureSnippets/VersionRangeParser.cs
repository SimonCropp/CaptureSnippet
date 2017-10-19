using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeParser
    {


        public static NuGetVersion ParseVersion(string stringVersion, string pretext = null)
        {
            Guard.AgainstEmpty(pretext, nameof(pretext));
            if (pretext == null)
            {
                if (NuGetVersion.TryParse(stringVersion, out var version))
                {
                    return version;
                }
            }
            else
            {
                if (NuGetVersion.TryParse(stringVersion, out var version))
                {
                    if (version.IsPrerelease)
                    {
                        throw new Exception($"Could parse version '{stringVersion}'. Cant mix pre-release from version with '{pretext}'.");
                    }

                    return new NuGetVersion(version.Major, version.Minor, version.Patch, new[] { pretext }, null);
                }
            }
            throw new Exception($"Could parse version '{stringVersion}'.");
        }


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
            if (int.TryParse(stringVersion, out var majorPart))
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
            var valueWithPre = $"{stringVersion}-{pretext}";
            if (NuGetVersion.TryParse(valueWithPre, out var minVersion))
            {
                parsedVersion = new VersionRange(
                    minVersion: minVersion,
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(minVersion.Major + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            var message = $"Could not use pretext:'{pretext}' to parse a SemanticVersion. Value attempted:'{valueWithPre}'.";
            throw new Exception(message);
        }

        static bool TryParseStable(string stringVersion, out VersionRange parsedVersion)
        {
            if (int.TryParse(stringVersion, out var majorPart))
            {
                parsedVersion = new VersionRange(
                    minVersion: new NuGetVersion(majorPart, 0, 0),
                    includeMinVersion: true,
                    maxVersion: new NuGetVersion(majorPart + 1, 0, 0),
                    includeMaxVersion: false);
                return true;
            }
            if (NuGetVersion.TryParse(stringVersion, out var minVersion))
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