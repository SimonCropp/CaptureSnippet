using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MethodTimer;

namespace CaptureSnippets
{
    /// <summary>
    /// Provides a higher level abstraction over snippets parsing
    /// </summary>
    public class CachedSnippetExtractor
    {
        ConvertSnippetPackageGroupToList convertToList;
        DirectorySnippetExtractor extractor;
        ConcurrentDictionary<string, CachedSnippets> cache = new ConcurrentDictionary<string, CachedSnippets>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extractData">The convention that is passed to <see cref="DirectorySnippetExtractor"/>.</param>
        /// <param name="includeDirectory">Directories to include.</param>
        /// <param name="includeFile">Files to include.</param>
        public CachedSnippetExtractor(ExtractPathData extractData, IncludeDirectory includeDirectory, IncludeFile includeFile, TranslatePackage translatePackage = null, ConvertSnippetPackageGroupToList convertToList = null)
        {
            this.convertToList = convertToList;
            Guard.AgainstNull(extractData, "extractData");
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            extractor = new DirectorySnippetExtractor(extractData, includeDirectory, includeFile, translatePackage);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        [Time]
        public bool TryRemoveDirectory(string directory, out CachedSnippets cached)
        {
            return cache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        [Time]
        public Task<CachedSnippets> FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            CachedSnippets cached;
            if (!cache.TryGetValue(directory, out cached))
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            if (cached.Ticks != lastDirectoryWrite)
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            return Task.FromResult(cached);
        }

        async Task<CachedSnippets> UpdateCache(string directory, long lastDirectoryWrite)
        {
            var readSnippets = await extractor.FromDirectory(directory);
            var snippetGroups = SnippetGrouper.Group(readSnippets.Snippets, convertToList);
            var cachedSnippets = new CachedSnippets(
                ticks: lastDirectoryWrite,
                readingErrors: readSnippets.GetSnippetsInError().ToList(),
                groupingErrors: snippetGroups.Errors,
                snippetGroups: snippetGroups.Groups);
            return cache[directory] = cachedSnippets;
        }

    }
}