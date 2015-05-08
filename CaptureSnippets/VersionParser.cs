namespace CaptureSnippets
{
    /// <summary>
    /// Parser for converting a string to a <see cref="Version"/>.
    /// </summary>
    public static class VersionParser
    {

        public static bool TryParseVersion(string stringVersion, out Version parsedVersion)
        {
            parsedVersion = null;
            var split = stringVersion.Split('.');
            if (split.Length == 1)
            {
                int major;
                if (!int.TryParse(split[0], out major))
                {
                    return false;
                }
                parsedVersion= new Version(major);
                return true;
            }
            if (split.Length == 2)
            {
                int major;
                if (!int.TryParse(split[0], out major))
                {
                    return false;
                }
                int minor;
                if (!int.TryParse(split[1], out minor))
                {
                    return false;
                }
                parsedVersion= new Version(major,minor);
                return true;
            }
            if (split.Length == 3)
            {
                int major;
                if (!int.TryParse(split[0], out major))
                {
                    return false;
                }
                int minor;
                if (!int.TryParse(split[1], out minor))
                {
                    return false;
                }
                int patch;
                if (!int.TryParse(split[2], out patch))
                {
                    return false;
                }
                parsedVersion= new Version(major,minor,patch);
                return true;
            }
            return false;
        }
    }
}