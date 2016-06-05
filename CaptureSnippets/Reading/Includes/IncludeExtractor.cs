using System.Collections.Generic;
using System.IO;
using System.Linq;
using MethodTimer;
using NuGet.Versioning;

namespace CaptureSnippets
{

    public class IncludeExtractor
    {
        ExtractIncludeData extractIncludeData;
        ExtractPathData extractPathData;
        TranslatePackage translatePackage;

        public IncludeExtractor(ExtractIncludeData extractIncludeData, ExtractPathData extractPathData, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(extractIncludeData, nameof(extractIncludeData));
            Guard.AgainstNull(extractPathData, nameof(extractPathData));
            this.extractIncludeData = extractIncludeData;
            this.extractPathData = extractPathData;
            if (translatePackage != null)
            {
                this.translatePackage = translatePackage;
            }
            else
            {
                this.translatePackage = alias => alias;
            }
        }

        [Time]
        public ReadIncludes FromDirectory(string directoryPath, VersionRange rootVersionRange = null, Package rootPackage = null, Component rootComponent = null)
        {
            Guard.AgainstNull(directoryPath, nameof(directoryPath));
            var list = InnerFromDirectory(directoryPath, rootVersionRange, rootPackage, rootComponent).ToList();
            return new ReadIncludes(list);
        }

        IEnumerable<ReadInclude> InnerFromDirectory(string directoryPath, VersionRange parentVersion, Package parentPackage, Component parentComponent)
        {
            VersionRange directoryVersion;
            Package directoryPackage;
            Component directoryComponent;
            PathDataExtractor.ExtractData(parentVersion, parentPackage, parentComponent, extractPathData, directoryPath, out directoryVersion, out directoryPackage, out directoryComponent);

            foreach (var file in Directory.EnumerateFiles(directoryPath, "*.include.md"))
            {
                yield return ReadInclude(file, directoryVersion, directoryPackage, directoryComponent);
            }
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath))
            {
                foreach (var readInclude in InnerFromDirectory(subDirectory, directoryVersion, directoryPackage, directoryComponent))
                {
                    yield return readInclude;
                }
            }
        }

        ReadInclude ReadInclude(string file, VersionRange parentVersion, Package parentPackage, Component parentComponent)
        {
            VersionRange fileVersion;
            Package filePackage;
            Component fileComponent;
            string key;
            IncludeDataExtractor.ExtractDataForFile(
                parentVersion: parentVersion,
                parentPackage: parentPackage,
                parentComponent: parentComponent,
                extractData: extractIncludeData,
                path: file,
                key: out key,
                version: out fileVersion,
                package: out filePackage,
                component: out fileComponent);

            filePackage = translatePackage(filePackage);
            return new ReadInclude(
                key: key,
                value: File.ReadAllText(file),
                path: file,
                version: fileVersion,
                package: filePackage,
                component: fileComponent);
        }
    }
}