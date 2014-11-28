using System;

namespace CaptureSnippets
{
    public class VersionParser
    {

        public static bool TryParseVersion(string stringVersion, out Version parsedVersion)
        {
            int intVersion;
            if (int.TryParse(stringVersion, out intVersion))
            {
                parsedVersion = new Version(intVersion, 0);
                return true;
            }
            return Version.TryParse(stringVersion, out parsedVersion);
        }
    }
}