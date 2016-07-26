using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="ReadSnippet"/>s from a given directory.
    /// </summary>
    public class DirectorySnippetExtractor
    {
        ExtractDirectoryPathData extractDirectoryPathData;
        DirectoryFilter directoryFilter;
        FileFilter fileFilter;
        FileSnippetExtractor fileExtractor;

        /// <summary>
        /// Initialise a new instance of <see cref="DirectorySnippetExtractor"/>.
        /// </summary>
        /// <param name="extractDirectoryPathData">How to extract a <see cref="PathData"/> from a given path.</param>
        /// <param name="fileFilter">Used to filter files.</param>
        /// <param name="directoryFilter">Used to filter directories.</param>
        public DirectorySnippetExtractor(ExtractDirectoryPathData extractDirectoryPathData, ExtractFileNameData extractFileNameData, DirectoryFilter directoryFilter, FileFilter fileFilter)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            Guard.AgainstNull(extractDirectoryPathData, nameof(extractDirectoryPathData));
            this.extractDirectoryPathData = extractDirectoryPathData;
            this.directoryFilter = directoryFilter;
            this.fileFilter = fileFilter;
            fileExtractor = new FileSnippetExtractor(extractFileNameData);
        }

        public ReadSnippets FromDirectory(string directoryPath, VersionRange rootVersionRange, Package rootPackage, Component rootComponent)
        {
            Guard.AgainstNull(directoryPath, nameof(directoryPath));
            Guard.AgainstNull(rootVersionRange, nameof(rootVersionRange));
            Guard.AgainstNull(rootPackage, nameof(rootPackage));
            Guard.AgainstNull(rootComponent, nameof(rootComponent));
            var snippets = new ConcurrentBag<ReadSnippet>();
            var files = new List<FileAndData>();
            FindFiles(directoryPath, rootVersionRange, rootPackage, rootComponent,files);
            Parallel.ForEach(files, data =>
            {
                FromFile(data.Path, data.DirectoryVersion, data.DirectoryPackage, data.DirectoryComponent, snippets.Add);
            });
            return new ReadSnippets(snippets.ToArray());
        }

        struct FileAndData
        {
            public string Path;
            public VersionRange DirectoryVersion;
            public Package DirectoryPackage;
            public Component DirectoryComponent;
        }

        void FindFiles(string directoryPath, VersionRange parentVersion, Package parentPackage, Component parentComponent, List<FileAndData> files)
        {
            VersionRange directoryVersion;
            Package directoryPackage;
            Component directoryComponent;
            var pathData = extractDirectoryPathData(directoryPath);
            PathDataExtractor.ExtractData(parentVersion, parentPackage, parentComponent, pathData, out directoryVersion, out directoryPackage, out directoryComponent);
            foreach (var file in Directory.EnumerateFiles(directoryPath)
                   .Where(s => fileFilter(s)))
            {
                files.Add(new FileAndData
                {
                    Path = file,
                    DirectoryVersion = directoryVersion,
                    DirectoryPackage = directoryPackage,
                    DirectoryComponent = directoryComponent
                });
            }
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(s => directoryFilter(s)))
            {
                FindFiles(subDirectory, directoryVersion, directoryPackage, directoryComponent, files);
            }
        }

        void FromFile(string file, VersionRange parentVersion, Package parentPackage, Component parentComponent, Action<ReadSnippet> callback)
        {
            using (var textReader = File.OpenText(file))
            {
                fileExtractor.AppendFromReader(textReader, file, parentVersion, parentPackage, parentComponent, callback);
            }
        }

    }
}