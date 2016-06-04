using System;
using NuGet.Versioning;

namespace CaptureSnippets
{
    static class IncludeDataExtractor
    {

        public static void ExtractDataForFile(VersionRange parentVersion, Package parentPackage, ExtractIncludeData extractData, string path, out string key, out VersionRange version, out Package package)
        {
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            IncludeDataExtractor.ExtractData(parentVersion, parentPackage, extractData, pathWithoutExtension, out key, out version, out package);
        }

        public static void ExtractData(VersionRange parentVersion, Package parentPackage, ExtractIncludeData extractData, string path, out string key, out VersionRange version, out Package package)
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