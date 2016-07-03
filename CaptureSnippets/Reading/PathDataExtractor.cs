using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class PathDataExtractor
    {

        public static void ExtractDataForFile(VersionRange parentVersion, Package parentPackage, Component parentComponent, ExtractFileNameData extractPathData, string path, out VersionRange version, out Package package, out Component component)
        {
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            var data = extractPathData(pathWithoutExtension);
            ExtractData(parentVersion, parentPackage, parentComponent, data, out version, out package, out component);
        }

        public static void ExtractData(VersionRange parentVersion, Package parentPackage, Component parentComponent, PathData data, out VersionRange version, out Package package, out Component component)
        {
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
                package = data.Package;
            }

            if (data.UseParentComponent)
            {
                component = parentComponent;
            }
            else
            {
                component = data.Component;
            }
        }
    }
}