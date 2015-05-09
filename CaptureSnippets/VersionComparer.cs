using System;
using System.Collections.Generic;

namespace CaptureSnippets
{
    public class VersionComparer: IComparer<Version>
    {
        public static VersionComparer Instance = new VersionComparer();
        public int Compare(Version version1, Version version2)
        {
            Guard.AgainstNull(version2, "version1");
            Guard.AgainstNull(version1, "version2");
            if (version2 == Version.ExplicitEmpty)
            {
                throw new ArgumentException("Cannot compare an ExplicitEmpty version.", "version2");
            }
            if (version1 == Version.ExplicitEmpty)
            {
                throw new ArgumentException("Cannot compare an ExplicitEmpty version.", "version1");
            }
            if (version1.Major != version2.Major)
            {
                return version1.Major > version2.Major ? 1 : -1;
            }
            if (version1.Minor != version2.Minor)
            {
                return version1.Minor > version2.Minor ? 1 : -1;
            }
            if (version1.Patch == version2.Patch)
            {
                return 0;
            }
            return version1.Patch > version2.Patch ? 1 : -1;
        }

    }
}