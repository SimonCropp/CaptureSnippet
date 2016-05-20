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
        ExtractMetaDataFromPath extractMetaDataFromPath;
        IncludeDirectory includeDirectory;
        IncludeFile includeFile;
        FileSnippetExtractor fileExtractor;

        /// <summary>
        /// Initialise a new instance of <see cref="DirectorySnippetExtractor"/>.
        /// </summary>
        /// <param name="extractMetaDataFromPath">How to extract a <see cref="SnippetMetaData"/> from a given path.</param>
        /// <param name="includeFile">Used to filter files.</param>
        /// <param name="includeDirectory">Used to filter directories.</param>
        /// <param name="parseVersion">Used to infer <see cref="VersionRange"/>. If null will default to <see cref="VersionRangeParser.TryParseVersion"/>.</param>
        public DirectorySnippetExtractor(ExtractMetaDataFromPath extractMetaDataFromPath, IncludeDirectory includeDirectory, IncludeFile includeFile, ParseVersion parseVersion = null)
        {
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            Guard.AgainstNull(extractMetaDataFromPath, "extractMetaData");
            this.extractMetaDataFromPath = extractMetaDataFromPath;
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
            fileExtractor = new FileSnippetExtractor(extractMetaDataFromPath, parseVersion);
        }

        [Time]
        public async Task<ReadSnippets> FromDirectory(string directoryPath)
        {
            Guard.AgainstNull(directoryPath, "directoryPath");
            var snippets = new ConcurrentBag<ReadSnippet>();
            await Task.WhenAll(FromDirectory(directoryPath, directoryPath, snippets.Add))
                .ConfigureAwait(false);
            var readOnlyList = snippets.ToList();
            return new ReadSnippets(readOnlyList);
        }


        IEnumerable<Task> FromDirectory(string rootPath, string directoryPath, Action<ReadSnippet> add)
        {
            var cache = new Dictionary<string, SnippetMetaData>();
            foreach (var subDirectory in Extensions.AllDirectories(directoryPath, includeDirectory))
            {
                var parent = Directory.GetParent(subDirectory).FullName;
                SnippetMetaData parentInfo;
                cache.TryGetValue(parent, out parentInfo);
                var metaData = extractMetaDataFromPath(rootPath, subDirectory, parentInfo);
                cache.Add(subDirectory, metaData);
                foreach (var file in Directory.EnumerateFiles(subDirectory)
                    .Where(s => includeFile(s)))
                {
                    yield return FromFile(rootPath, file, metaData, add);
                }
            }
        }

        async Task FromFile(string rootPath, string file, SnippetMetaData metaData, Action<ReadSnippet> callback)
        {
            using (var textReader = File.OpenText(file))
            {
                await fileExtractor.AppendFromReader(textReader, rootPath, file, metaData, callback)
                    .ConfigureAwait(false);
            }
        }

    }
}