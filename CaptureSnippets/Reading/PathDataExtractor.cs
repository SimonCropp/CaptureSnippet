using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class PathDataExtractor
    {

        public static void ExtractDataForFile(VersionRange parentVersion, Package parentPackage, ExtractPathData extractPathData, string path, out VersionRange version, out Package package)
        {
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            ExtractData(parentVersion, parentPackage, extractPathData, pathWithoutExtension, out version, out package);
        }

        public static void ExtractData(VersionRange parentVersion, Package parentPackage, ExtractPathData extractPathData, string path, out VersionRange version, out Package package)
        {
            var data = extractPathData(path);
            if (data == null)
            {
                throw new Exception("ExtractPathData cannot return null.");
            }
            if (data.UseParentVersion)
            {
                version = parentVersion;
            }
            else
            {
                if (data.Version == null)
                {
                    throw new Exception("Null version not allowed.");
                }
                version = data.Version;
            }
            if (data.UseParentPackage)
            {
                package = parentPackage;
            }
            else
            {
                if (data.Package == null)
                {
                    throw new Exception("Null package not allowed.");
                }
                package = data.Package;
            }
        }
    }
}