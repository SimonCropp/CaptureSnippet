using System;
using System.Collections.Generic;

namespace CaptureSnippets
{
    public class VersionComparer: IComparer<Version>
    {
        public static VersionComparer Instance = new VersionComparer();
        public int Compare(Version version1, Version version2)
        {
            Guard.AgainstNull(version1, "version1");
            Guard.AgainstNull(version2, "version2");
            if (version1 == Version.ExplicitEmpty)
            {
                throw new ArgumentException("Cannot compare an ExplicitEmpty version.", "version1");
            }
            if (version2 == Version.ExplicitEmpty)
            {
                throw new ArgumentException("Cannot compare an ExplicitEmpty version.", "version2");
            }
            if (version2.Major != version1.Major)
            {
                return version2.Major > version1.Major ? 1 : -1;
            }
            if (version2.Minor != version1.Minor)
            {
                return version2.Minor > version1.Minor ? 1 : -1;
            }
            if (version2.Patch == version1.Patch)
            {
                return 0;
            }
            return version2.Patch > version1.Patch ? 1 : -1;
        }

    }
}