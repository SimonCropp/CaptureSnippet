using System.Collections.Generic;
using System.IO;
using System.Linq;
using MethodTimer;
using NuGet.Versioning;

namespace CaptureSnippets
{

    public class IncludeExtractor
    {
        ExtractVersionAndPackageFromPath extractVersionAndPackage;

        public IncludeExtractor(ExtractVersionAndPackageFromPath extractVersionAndPackage)
        {
            Guard.AgainstNull(extractVersionAndPackage, "extractMetaData");
            this.extractVersionAndPackage = extractVersionAndPackage;
        }

        [Time]
        public ReadIncludes FromDirectory(string directoryPath, VersionRange rootVersionRange = null, Package rootPackage = null)
        {
            Guard.AgainstNull(directoryPath, "directoryPath");
            var list = InnerFromDirectory(directoryPath, rootVersionRange, rootPackage).ToList();
            return new ReadIncludes(list);
        }

        IEnumerable<ReadInclude> InnerFromDirectory(string directoryPath, VersionRange parentVersion, Package parentPackage)
        {
            VersionRange directoryVersion;
            Package directoryPackage;
            VersionAndPackageExtractor.ExtractVersionAndPackage(parentVersion, parentPackage, extractVersionAndPackage, directoryPath, out directoryVersion, out directoryPackage);

            foreach (var file in Directory.EnumerateFiles(directoryPath, "*.include.md"))
            {
                yield return ReadInclude(file, directoryVersion, directoryPackage);
            }
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath))
            {
                foreach (var readInclude in InnerFromDirectory(subDirectory, directoryVersion, directoryPackage))
                {
                    yield return readInclude;
                }
            }
        }

        ReadInclude ReadInclude(string file, VersionRange parentVersion, Package parentPackage)
        {
            var key = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file))
                .ToLowerInvariant();
            VersionRange fileVersion;
            Package filePackage;
            VersionAndPackageExtractor.ExtractVersionAndPackageForFile(
                parentVersion: parentVersion,
                parentPackage: parentPackage,
                extractVersionAndPackage: extractVersionAndPackage,
                path: file,
                version: out fileVersion,
                package: out filePackage);
            return new ReadInclude(
                key: key,
                value: File.ReadAllText(file),
                path: file,
                version: fileVersion,
                package: filePackage);
        }
    }
}