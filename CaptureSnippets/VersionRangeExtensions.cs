using NuGet.Versioning;

namespace CaptureSnippets
{
    static class VersionRangeExtensions
    {


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

        public static bool CanMerge(VersionRange range1, VersionRange range2, out VersionRange newVersion)
        {
            if (range1.Equals(VersionRange.All) || range2.Equals(VersionRange.All))
            {
                newVersion = VersionRange.All;
                return true;
            }

            if (range1.IsMaxInclusive &&
                range2.IsMinInclusive && 
                range1.MaxVersion.Equals(range2.MinVersion))
            {
                newVersion = new VersionRange(
                    minVersion: range1.MinVersion,
                    includeMinVersion: range1.IsMinInclusive,
                    maxVersion: range2.MaxVersion,
                    includeMaxVersion: range2.IsMaxInclusive);
                return true;
            }
            if (range1.IsMinInclusive && 
                range2.IsMaxInclusive && 
                range1.MinVersion.Equals(range2.MaxVersion))
            {
                newVersion = new VersionRange(
                    maxVersion: range1.MaxVersion,
                    includeMinVersion: range2.IsMinInclusive,
                    minVersion: range2.MinVersion,
                    includeMaxVersion: range1.IsMaxInclusive);
                return true;
            }
            if (range1.OverlapsWith(range2))
            {
                bool maxInclusive;
                SimpleVersion maxVersion;
                MaxVersion(range1, range2, out maxVersion, out maxInclusive);
                
                SimpleVersion minVersion;
                bool minInclusive;
                MinVersion(range1,range2, out minVersion, out minInclusive);
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

            newVersion = null;
            return false;
        }

        public static void MaxVersion(VersionRange range1, VersionRange range2, out SimpleVersion simpleVersion, out bool isMaxInclusive)
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

        public static void MinVersion(VersionRange range1, VersionRange range2, out SimpleVersion simpleVersion, out bool isMinInclusive)
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

        public static bool OverlapsWith(this VersionRange range1, VersionRange range2)
        {
            if (range1.MinVersion == null || range2.MaxVersion == null)
            {
                return range2.MinVersion < range1.MaxVersion;
            }
            if (range1.MaxVersion == null || range2.MinVersion == null)
            {
                return range1.MinVersion < range2.MaxVersion;
            }
            return
                (range1.MinVersion < range2.MaxVersion) &&
                range2.MinVersion < range1.MaxVersion;
        }
    }
}