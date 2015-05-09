using System.Collections.Generic;

namespace CaptureSnippets
{
    class VersionEquator : IEqualityComparer<Version>
    {
        public static VersionEquator Instance = new VersionEquator();
        internal static bool Equals(Version version1, Version version2)
        {
            if (version1 == version2)
            {
                return true;
            }
            if (version1 == null)
            {
                return false;
            }
            if (version2 == null)
            {
                return false;
            }
            return version2.Major == version1.Major && (version2.Minor == version1.Minor) && version2.Patch == version1.Patch;
        }

        public int GetHashCode(Version version)
        {
            if (version == Version.ExplicitNull)
            {
                return 0;
            }
            return 0 | (version.Major & 15) << 28 | (version.Minor.GetValueOrDefault(-1) & byte.MaxValue) << 20 | version.Patch.GetValueOrDefault(-1) & 4095;

        }

        bool IEqualityComparer<Version>.Equals(Version x, Version y)
        {
            return Equals(x, y);
        }
    }
}