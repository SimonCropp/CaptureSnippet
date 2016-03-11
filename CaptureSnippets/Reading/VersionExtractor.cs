using NuGet.Versioning;

namespace CaptureSnippets
{
    public delegate VersionRange VersionExtractor(string path, VersionRange parent);
}