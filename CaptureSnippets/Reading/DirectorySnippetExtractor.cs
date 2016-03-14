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
        ExtractVersion extractVersion;
        ExtractPackage extractPackage;
        DirectoryIncluder includeDirectory;
        FileIncluder includeFile;
        FileSnippetExtractor fileExtractor;

        /// <summary>
        /// Initialise a new instance of <see cref="DirectorySnippetExtractor"/>.
        /// </summary>
        /// <param name="extractVersion">How to extract a <see cref="VersionRange"/> from a given path.</param>
        /// <param name="extractPackage">How to extract a package from a given file path. Return null for unknown.</param>
        public DirectorySnippetExtractor(ExtractVersion extractVersion, ExtractPackage extractPackage, DirectoryIncluder includeDirectory, FileIncluder includeFile)
        {
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            Guard.AgainstNull(extractVersion, "extractVersion");
            Guard.AgainstNull(extractPackage, "extractPackage");
            this.extractVersion = extractVersion;
            this.extractPackage = extractPackage;
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
            fileExtractor = new FileSnippetExtractor(extractVersion, extractPackage);
        }

        [Time]
        public async Task<ReadSnippets> FromDirectory(string directoryPath)
        {
            Guard.AgainstNull(directoryPath, "directoryPath");
            var snippets = new ConcurrentBag<ReadSnippet>();
            await Task.WhenAll(FromDirectory(directoryPath, snippets.Add))
                .ConfigureAwait(false);
            var readOnlyList = snippets.ToList();
            return new ReadSnippets(readOnlyList);
        }

        class VersionAndPackage
        {
            public string Package;
            public VersionRange Version;
        }
        IEnumerable<Task> FromDirectory(string directoryPath, Action<ReadSnippet> add)
        {
            var cache = new Dictionary<string, VersionAndPackage>();
            foreach (var subDirectory in Extensions.AllDirectories(directoryPath, includeDirectory)
                .Where(s => includeDirectory(s)))
            {
                var parent = Directory.GetParent(subDirectory).FullName;
                VersionAndPackage parentInfo;
                cache.TryGetValue(parent, out parentInfo);
                var package = extractPackage(subDirectory, parentInfo?.Package);
                var version = extractVersion(subDirectory, parentInfo?.Version);
                cache.Add(subDirectory, new VersionAndPackage
                {
                    Version = version,
                    Package = package
                });
                foreach (var file in Directory.EnumerateFiles(subDirectory)
                    .Where(s => includeFile(s)))
                {
                    yield return FromFile(file, package, version, add);
                }
            }
        }

        async Task FromFile(string file, string parentPackage, VersionRange parentVersion, Action<ReadSnippet> callback)
        {
            using (var textReader = File.OpenText(file))
            {
                await fileExtractor.AppendFromReader(textReader, file, parentVersion, parentPackage, callback)
                    .ConfigureAwait(false);
            }
        }

    }
}