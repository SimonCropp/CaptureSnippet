using System.Collections.Generic;
using System.IO;
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
        VersionExtractor versionExtractor;
        PackageExtractor packageExtractor;
        DirectoryIncluder includeDirectory;
        FileIncluder includeFile;

        /// <summary>
        /// Initialise a new instance of <see cref="DirectorySnippetExtractor"/>.
        /// </summary>
        /// <param name="versionExtractor">How to extract a <see cref="VersionRange"/> from a given path.</param>
        /// <param name="packageExtractor">How to extract a package from a given file path. Return null for unknown.</param>
        public DirectorySnippetExtractor(VersionExtractor versionExtractor, PackageExtractor packageExtractor, DirectoryIncluder includeDirectory, FileIncluder includeFile)
        {
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            Guard.AgainstNull(versionExtractor, "versionExtractor");
            Guard.AgainstNull(packageExtractor, "packageExtractor");
            this.versionExtractor = versionExtractor;
            this.packageExtractor = packageExtractor;
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
        }

        [Time]
        public async Task<ReadSnippets> FromDirectory(string directoryPath)
        {
            Guard.AgainstNull(directoryPath, "directoryPath");
            var readSnippets = new List<ReadSnippet>();
            var snippetExtractor = new FileSnippetExtractor(readSnippets, versionExtractor, packageExtractor);
            await FromDirectory(directoryPath, snippetExtractor, null, null);
            return new ReadSnippets(readSnippets);
        }

        async Task FromDirectory(string directoryPath, FileSnippetExtractor fileExtractor, string parentPackage, VersionRange parentVersion)
        {
            var package = packageExtractor(directoryPath, parentPackage);
            var version = versionExtractor(directoryPath, parentVersion);
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath))
            {
                if (includeDirectory(subDirectory))
                {
                    await FromDirectory(subDirectory, fileExtractor, package, version);
                }
            }
            foreach (var file in Directory.EnumerateFiles(directoryPath))
            {
                if (includeFile(file))
                {
                    await FromFile(file, fileExtractor, package, version);
                }
            }
        }

        async Task FromFile(string file, FileSnippetExtractor fileExtractor, string parentPackage, VersionRange parentVersion)
        {
            using (var textReader = File.OpenText(file))
            {
                await fileExtractor.AppendFromReader(textReader, file, parentVersion, parentPackage)
                    .ConfigureAwait(false);
            }
        }

    }
}