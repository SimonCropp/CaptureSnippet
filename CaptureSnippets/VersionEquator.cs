namespace CaptureSnippets
{
    static class VersionEquator
    {

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
    }
}