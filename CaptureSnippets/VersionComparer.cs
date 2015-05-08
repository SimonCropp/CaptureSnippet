namespace CaptureSnippets
{
    static class VersionComparer
    {
        internal static int Compare(Version version1, Version version2)
        {
            if (version1 == null)
                return 1;
            if (version2.Major != version1.Major)
                return version2.Major > version1.Major ? 1 : -1;
            if (version2.Minor != version1.Minor)
                return version2.Minor > version1.Minor ? 1 : -1;
            if (version2.Patch == version1.Patch)
                return 0;
            return version2.Patch > version1.Patch ? 1 : -1;
        }

    }
}