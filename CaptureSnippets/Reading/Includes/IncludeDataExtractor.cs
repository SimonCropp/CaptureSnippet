using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class IncludeDataExtractor
    {

        public static void ExtractDataForFile(VersionRange parentVersion, Package parentPackage, Component parentComponent, ExtractIncludeData extractData, string path, out string key, out VersionRange version, out Package package, out Component component)
        {
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            ExtractData(parentVersion, parentPackage, parentComponent, extractData, pathWithoutExtension, out key, out version, out package, out component);
        }

        public static void ExtractData(VersionRange parentVersion, Package parentPackage, Component parentComponent, ExtractIncludeData extractData, string path, out string key, out VersionRange version, out Package package, out Component component)
        {
            var data = extractData(path);
            if (data == null)
            {
                throw new Exception("ExtractIncludeDataFromPath cannot return null.");
            }
            if (string.IsNullOrWhiteSpace(data.Key))
            {
                throw new Exception("ExtractIncludeDataFromPath cannot return an empty or null key.");
            }
            key = data.Key;

            if (data.UseParentComponent)
            {
                component = parentComponent;
            }
            else
            {
                if (data.Component == null)
                {
                    throw new Exception("Null component not allowed.");
                }
                component = data.Component;
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