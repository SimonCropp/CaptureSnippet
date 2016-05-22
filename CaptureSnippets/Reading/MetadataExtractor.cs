using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class MetadataExtractor
    {
        public static void ExtractVersionAndPackage(string rootPath, VersionRange parentVersion, Package parentPackage, ExtractMetaDataFromPath metaDataFromPath, string path, out VersionRange version, out Package package)
        {
            var metaDataForPath = metaDataFromPath(rootPath, path);
            if (metaDataForPath == null)
            {
                throw new Exception("ExtractMetaDataFromPath cannot return null.");
            }
            if (metaDataForPath.UseParentVersion)
            {
                version = parentVersion;
            }
            else
            {
                version = metaDataForPath.Version;
            }
            if (metaDataForPath.UseParentPackage)
            {
                package = parentPackage;
            }
            else
            {
                package = metaDataForPath.Package;
            }
        }
    }
}