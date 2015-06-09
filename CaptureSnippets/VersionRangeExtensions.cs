using System;
using System.Linq;
using System.Text;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public static class VersionRangeExtensions
    {
        public static SemanticVersion Semantic(this SimpleVersion version)
        {
            return (SemanticVersion)version;
        }
        internal static SimpleVersion VersionForCompare(this VersionRange range)
        {
            if (range.MinVersion == null)
            {
                return range.MaxVersion;
            }
            return range.MinVersion;
        }


        public static string NextVersion(this SemanticVersion version)
        {
            if (version.Patch > 0)
            {
                return string.Format("{0}.{1}.{2}.x", version.Major, version.Minor, version.Patch + 1);
            }
            if (version.Minor > 0)
            {
                return string.Format("{0}.{1}.1.x", version.Major, version.Minor);
            }
            return string.Format("{0}.1.x", version.Major);
        }

        public static string PreviousVersion(this SemanticVersion version)
        {
            if (version.Patch > 0)
            {
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Patch - 1);
            }
            if (version.Minor > 0)
            {
                return string.Format("{0}.{1}.x", version.Major, version.Minor - 1);
            }
            return string.Format("{0}.x", version.Major - 1);
        }

        public static string SimplePrint(this SimpleVersion version)
        {
            var semantic = version.Semantic();
            if (semantic.Patch > 0)
            {
                if (semantic.IsPrerelease)
                {
                    return string.Format("{0}.{1}.{2}-{3}", semantic.Major, semantic.Minor, semantic.Patch, semantic.ReleaseLabels.First());
                }
                return string.Format("{0}.{1}.{2}.x", semantic.Major, semantic.Minor, semantic.Patch);
            }
            if (semantic.Minor > 0)
            {
                if (semantic.IsPrerelease)
                {
                    return string.Format("{0}.{1}-{2}", semantic.Major, semantic.Minor, semantic.ReleaseLabels.First());
                }
                return string.Format("{0}.{1}.x", semantic.Major, semantic.Minor);
            }
            if (semantic.IsPrerelease)
            {
                return string.Format("{0}-{1}", semantic.Major, semantic.ReleaseLabels.First());
            }
            return string.Format("{0}.x", semantic.Major);
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
            var minVersion = (SemanticVersion)range.MinVersion;
            var maxVersion = (SemanticVersion)range.MaxVersion;

            if (range.HasUpperBound && maxVersion.IsPrerelease)
            {
                var message = string.Format("Pre release not allowed on upper bound '{0}.", range);
                throw new Exception(message);
            }
            if (range.HasLowerBound && minVersion.IsPrerelease && !range.IsMinInclusive)
            {
                var message = string.Format("Pre release not allowed on non-inclusive lower bound '{0}.", range);
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
            }

            // single version
            if (range.HasLowerAndUpperBounds && maxVersion.Equals(minVersion) && range.IsMinInclusive && range.IsMaxInclusive)
            {
                return minVersion.SimplePrint();
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

            sb.Append(" - ");

            if (range.HasUpperBound)
            {
                if (range.IsMaxInclusive)
                {
                    sb.Append(maxVersion.SimplePrint());
                }
                else
                {
                    sb.Append(maxVersion.PreviousVersion());
                }
            }
            else
            {
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
                SimpleVersion maxVersion;
                MaxVersion(range1, range2, out maxVersion, out maxInclusive);

                SimpleVersion minVersion;
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

        internal static void MaxVersion(VersionRange range1, VersionRange range2, out SimpleVersion simpleVersion, out bool isMaxInclusive)
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
                simpleVersion =  range1.MaxVersion;
                isMaxInclusive = range1.IsMaxInclusive;
                return;
            }
            simpleVersion = range2.MaxVersion;
            isMaxInclusive = range2.IsMaxInclusive;
        }

        internal static void MinVersion(VersionRange range1, VersionRange range2, out SimpleVersion simpleVersion, out bool isMinInclusive)
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

        static bool CompareVersions(SimpleVersion version1, SimpleVersion version2)
        {
            if (version1 == null || version2 == null)
            {
                return true;
            }
            return version1 < version2;
        }
    }
}