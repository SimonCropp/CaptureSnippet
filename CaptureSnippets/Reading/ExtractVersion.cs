using NuGet.Versioning;

namespace CaptureSnippets
{
    public delegate VersionRange ExtractVersion(string fileOrDirectoryPath, VersionRange parent);
}