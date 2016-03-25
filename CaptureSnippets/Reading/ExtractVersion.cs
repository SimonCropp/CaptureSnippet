using NuGet.Versioning;

namespace CaptureSnippets
{
    public delegate Result<VersionRange> ExtractVersion(string fileOrDirectoryPath, VersionRange parent);
}