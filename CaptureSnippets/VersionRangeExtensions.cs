using System;
using System.Linq;
using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeExtensions
    {
        internal static NuGetVersion VersionForCompare(this VersionRange range)
        {
            if (range.MinVersion == null)
            {
                return range.MaxVersion;
            }
            return range.MinVersion;
        }


        public static string NextVersion(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot increment prerelease version");
            }
            if (version.Patch > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Patch + 1}.x";
            }
            if (version.Minor > 0)
            {
                return $"{version.Major}.{version.Minor}.1.x";
            }
            return $"{version.Major}.1.x";
        }

        public static string PreviousVersion(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot decrement pre-release version");
            }
            if (version.Patch > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Patch - 1}";
            }
            if (version.Minor > 0)
            {
                return $"{version.Major}.{version.Minor - 1}.x";
            }
            return $"{version.Major - 1}.x";
        }
        public static SemanticVersion PreviousVersion2(this SemanticVersion version)
        {
            if (version.IsPrerelease)
            {
                throw new Exception("Cannot decrement pre-release version");
            }
            if (version.Patch > 0)
            {
                return new SemanticVersion(version.Major,version.Minor,version.Patch - 1);
            }
            if (version.Minor > 0)
            {
                return new SemanticVersion(version.Major, version.Minor-1, 0);
            }
            return new SemanticVersion(version.Major-1, 0, 0);
        }

        public static string SimplePrint(this NuGetVersion version)
        {
            if (version.Patch > 0)
            {
                if (version.IsPrerelease)
                {
                    return $"{version.Major}.{version.Minor}.{version.Patch}-{version.ReleaseLabels.First()}";
                }
                return $"{version.Major}.{version.Minor}.{version.Patch}.x";
            }
            if (version.Minor > 0)
            {
                if (version.IsPrerelease)
                {
                    return $"{version.Major}.{version.Minor}-{version.ReleaseLabels.First()}";
                }
                return $"{version.Major}.{version.Minor}.x";
            }
            if (version.IsPrerelease)
            {
                return $"{version.Major}-{version.ReleaseLabels.First()}";
            }
            return $"{version.Major}.x";
        }

        public static string SimplePrint(this VersionRange range)
        {
            if (range.Equals(VersionRange.All) || range.Equals(VersionRange.AllStable))
            {
                return "All";
            }
            if (range.Equals(VersionRange.None))
            {
                return "None";
            }
            var minVersion = range.MinVersion;
            var maxVersion = range.MaxVersion;

            if (range.HasUpperBound && maxVersion.IsPrerelease)
            {
                var message = $"Pre release not allowed on upper bound '{range}'.";
                throw new Exception(message);
            }
            if (range.HasLowerBound && minVersion.IsPrerelease && !range.IsMinInclusive)
            {
                var message = $"Pre release not allowed on non-inclusive lower bound '{range}'.";
                throw new Exception(message);
            }
            if (range.HasLowerAndUpperBounds)
            {
                if (minVersion.Minor == 0 &&
                    minVersion.Patch == 0 &&
                    maxVersion.Minor == 0 &&
                    maxVersion.Patch == 0)
                {
                    if (range.IsMinInclusive &&
                        !range.IsMaxInclusive &&
                        minVersion.Major + 1 == maxVersion.Major)
                    {
                        return minVersion.SimplePrint();
                    }
                }


                // single version
                if (maxVersion.Equals(minVersion) && range.IsMinInclusive && range.IsMaxInclusive)
                {
                    return minVersion.SimplePrint();
                }
                // TODO:
                //if (minVersion.Equals(maxVersion.p) && range.IsMinInclusive && range.IsMaxInclusive)
                //{
                //    return minVersion.SimplePrint();
                //}
            }

            var sb = new StringBuilder();
            // normal range

            if (range.HasLowerBound)
            {
                if (range.IsMinInclusive)
                {
                    sb.Append(minVersion.SimplePrint());
                }
                else
                {
                    sb.Append(minVersion.NextVersion());
                }
            }
            else
            {
                sb.Append("N");
            }


            if (range.HasUpperBound)
            {
                if (range.IsMaxInclusive)
                {
                    sb.Append(" - ");
                    sb.Append(maxVersion.SimplePrint());
                }
                else
                {
                    var previousVersion2 = maxVersion.PreviousVersion2();
                    if (previousVersion2 > minVersion)
                    {
                        sb.Append(" - ");
                        sb.Append(maxVersion.PreviousVersion());
                    }
                }
            }
            else
            {
                sb.Append(" - ");
                sb.Append("N");
            }

            return sb.ToString();
        }

        public static string ToFriendlyString(this VersionRange version)
        {
            if (version.Equals(VersionRange.All))
            {
                return "all";
            }
            if (version.Equals(VersionRange.None))
            {
                return "none";
            }
            return version.PrettyPrint()
                .TrimStart('(')
                .TrimEnd(')');
        }

        internal static bool CanMerge(VersionRange range1, VersionRange range2, out VersionRange newVersion)
        {
            if (range1.Equals(VersionRange.All) || range2.Equals(VersionRange.All))
            {
                newVersion = VersionRange.All;
                return true;
            }

            if (range1.IncludePrerelease || range2.IncludePrerelease)
            {
                newVersion = null;
                return false;
            }
            if (range1.OverlapsWith(range2))
            {
                bool maxInclusive;
                NuGetVersion maxVersion;
                MaxVersion(range1, range2, out maxVersion, out maxInclusive);

                NuGetVersion minVersion;
                bool minInclusive;
                MinVersion(range1, range2, out minVersion, out minInclusive);
                if (minVersion == null && maxVersion == null)
                {
                    newVersion = VersionRange.All;
                    return true;
                }
                newVersion = new VersionRange(
                    minVersion: minVersion,
                    includeMinVersion: minInclusive,
                    maxVersion: maxVersion,
                    includeMaxVersion: maxInclusive);
                return true;
            }

            if ((range1.IsMaxInclusive || range2.IsMinInclusive) &&
                range1.HasUpperBound &&
                range1.MaxVersion.Equals(range2.MinVersion))
            {
                newVersion = new VersionRange(
                    minVersion: range1.MinVersion,
                    includeMinVersion: range1.IsMinInclusive,
                    maxVersion: range2.MaxVersion,
                    includeMaxVersion: range2.IsMaxInclusive);
                return true;
            }
            if ((range1.IsMinInclusive || range2.IsMaxInclusive) &&
                range1.HasLowerBound &&
                range1.MinVersion.Equals(range2.MaxVersion))
            {
                newVersion = new VersionRange(
                    maxVersion: range1.MaxVersion,
                    includeMinVersion: range2.IsMinInclusive,
                    minVersion: range2.MinVersion,
                    includeMaxVersion: range1.IsMaxInclusive);
                return true;
            }

            newVersion = null;
            return false;
        }

        internal static void MaxVersion(VersionRange range1, VersionRange range2, out NuGetVersion simpleVersion, out bool isMaxInclusive)
        {
            if (range1.MaxVersion == null)
            {
                simpleVersion = null;
                isMaxInclusive = range1.IsMaxInclusive;
                return;
            }
            if (range2.MaxVersion == null)
            {
                simpleVersion = null;
                isMaxInclusive = range2.IsMaxInclusive;
                return;
            }
            if (range1.MaxVersion > range2.MaxVersion)
            {
                simpleVersion = range1.MaxVersion;
                isMaxInclusive = range1.IsMaxInclusive;
                return;
            }
            simpleVersion = range2.MaxVersion;
            isMaxInclusive = range2.IsMaxInclusive;
        }

        internal static void MinVersion(VersionRange range1, VersionRange range2, out NuGetVersion simpleVersion, out bool isMinInclusive)
        {
            if (range1.MinVersion == null)
            {
                simpleVersion = null;
                isMinInclusive = range1.IsMinInclusive;
                return;
            }
            if (range2.MinVersion == null)
            {
                simpleVersion = null;
                isMinInclusive = range2.IsMinInclusive;
                return;
            }
            if (range1.MinVersion < range2.MinVersion)
            {
                simpleVersion = range1.MinVersion;
                isMinInclusive = range1.IsMinInclusive;
                return;
            }
            simpleVersion = range2.MinVersion;
            isMinInclusive = range2.IsMinInclusive;
        }

        internal static bool OverlapsWith(this VersionRange range1, VersionRange range2)
        {
            return
                CompareVersions(range1.MinVersion, range2.MaxVersion) &&
                CompareVersions(range2.MinVersion, range1.MaxVersion);
        }

        static bool CompareVersions(NuGetVersion version1, NuGetVersion version2)
        {
            if (version1 == null || version2 == null)
            {
                return true;
            }
            return version1 < version2;
        }
    }
}