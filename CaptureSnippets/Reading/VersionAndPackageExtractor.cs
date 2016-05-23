using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class VersionAndPackageExtractor
    {

        public static void ExtractVersionAndPackageForFile(VersionRange parentVersion, Package parentPackage, ExtractVersionAndPackageFromPath extractVersionAndPackage, string path, out VersionRange version, out Package package)
        {
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            VersionAndPackageExtractor.ExtractVersionAndPackage(parentVersion, parentPackage, extractVersionAndPackage, pathWithoutExtension, out version, out package);
        }

        public static void ExtractVersionAndPackage(VersionRange parentVersion, Package parentPackage, ExtractVersionAndPackageFromPath extractVersionAndPackage, string path, out VersionRange version, out Package package)
        {
            var metaDataForPath = extractVersionAndPackage(path);
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
                if (metaDataForPath.Version == null)
                {
                    throw new Exception("Null version not allowed.");
                }
                version = metaDataForPath.Version;
            }
            if (metaDataForPath.UseParentPackage)
            {
                package = parentPackage;
            }
            else
            {
                if (metaDataForPath.Package == null)
                {
                    throw new Exception("Null package not allowed.");
                }
                package = metaDataForPath.Package;
            }
        }
    }
}