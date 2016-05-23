using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MethodTimer;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="ReadSnippet"/>s from a given directory.
    /// </summary>
    public class DirectorySnippetExtractor
    {
        ExtractVersionAndPackageFromPath extractVersionAndPackageFromPath;
        IncludeDirectory includeDirectory;
        IncludeFile includeFile;
        FileSnippetExtractor fileExtractor;

        /// <summary>
        /// Initialise a new instance of <see cref="DirectorySnippetExtractor"/>.
        /// </summary>
        /// <param name="extractVersionAndPackageFromPath">How to extract a <see cref="VersionAndPackage"/> from a given path.</param>
        /// <param name="includeFile">Used to filter files.</param>
        /// <param name="includeDirectory">Used to filter directories.</param>
        public DirectorySnippetExtractor(ExtractVersionAndPackageFromPath extractVersionAndPackageFromPath, IncludeDirectory includeDirectory, IncludeFile includeFile, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            Guard.AgainstNull(extractVersionAndPackageFromPath, "extractMetaData");
            this.extractVersionAndPackageFromPath = extractVersionAndPackageFromPath;
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
            fileExtractor = new FileSnippetExtractor(extractVersionAndPackageFromPath, translatePackage);
        }

        [Time]
        public async Task<ReadSnippets> FromDirectory(string directoryPath, VersionRange rootVersionRange = null, Package rootPackage = null)
        {
            Guard.AgainstNull(directoryPath, "directoryPath");
            var snippets = new ConcurrentBag<ReadSnippet>();
            await Task.WhenAll(FromDirectory(directoryPath, rootVersionRange, rootPackage, snippets.Add))
                .ConfigureAwait(false);
            return new ReadSnippets(snippets.ToList());
        }


        IEnumerable<Task> FromDirectory(string directoryPath, VersionRange parentVersion, Package parentPackage, Action<ReadSnippet> add)
        {
            VersionRange directoryVersion;
            Package directoryPackage;
            VersionAndPackageExtractor.ExtractVersionAndPackage(parentVersion, parentPackage, extractVersionAndPackageFromPath, directoryPath, out directoryVersion, out directoryPackage);
            foreach (var file in Directory.EnumerateFiles(directoryPath)
                   .Where(s => includeFile(s)))
            {
                yield return FromFile(file, directoryVersion, directoryPackage, add);
            }
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(s => includeDirectory(s)))
            {
                foreach (var task in FromDirectory(subDirectory, directoryVersion, directoryPackage, add))
                {
                    yield return task;
                }
            }
        }

        async Task FromFile(string file, VersionRange parentVersion, Package parentPackage, Action<ReadSnippet> callback)
        {
            using (var textReader = File.OpenText(file))
            {
                await fileExtractor.AppendFromReader(textReader, file, parentVersion, parentPackage, callback)
                    .ConfigureAwait(false);
            }
        }

    }
}