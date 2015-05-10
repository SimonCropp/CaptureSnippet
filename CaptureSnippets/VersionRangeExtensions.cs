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
            if (version.MaxVersion == null)
            {
                return version.MinVersion.ToString();
            }
            return version.ToString();
        }
    }
}