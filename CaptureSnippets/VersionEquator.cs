namespace CaptureSnippets
{
    static class VersionEquator
    {

        internal static bool Equals(Version obj, Version version)
        {
            return obj != null && version.Major == obj.Major && (version.Minor == obj.Minor) && version.Patch == obj.Patch;
        }
    }
}