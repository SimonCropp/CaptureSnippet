using NuGet.Versioning;

namespace CaptureSnippets
{
    public delegate Result<VersionRange> ParseVersion(string version, string path, SnippetMetaData metaDataForParentPath);
}