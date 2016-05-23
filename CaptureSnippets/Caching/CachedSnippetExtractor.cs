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
        ConvertSnippetPackageGroupToList convertSnippetPackageGroupToList;
        DirectorySnippetExtractor snippetExtractor;
        ConcurrentDictionary<string, CachedSnippets> directoryToSnippets = new ConcurrentDictionary<string, CachedSnippets>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extractVersionAndPackageFromPath">The convention that is passed to <see cref="DirectorySnippetExtractor"/>.</param>
        /// <param name="includeDirectory">Directories to include.</param>
        /// <param name="includeFile">Files to include.</param>
        public CachedSnippetExtractor(ExtractVersionAndPackageFromPath extractVersionAndPackageFromPath, IncludeDirectory includeDirectory, IncludeFile includeFile, TranslatePackage translatePackage = null, ConvertSnippetPackageGroupToList convertSnippetPackageGroupToList = null)
        {
            this.convertSnippetPackageGroupToList = convertSnippetPackageGroupToList;
            Guard.AgainstNull(extractVersionAndPackageFromPath, "extractMetaData");
            Guard.AgainstNull(includeDirectory, "includeDirectory");
            Guard.AgainstNull(includeFile, "includeFile");
            snippetExtractor = new DirectorySnippetExtractor(extractVersionAndPackageFromPath, includeDirectory, includeFile, translatePackage);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        [Time]
        public bool TryRemoveDirectory(string directory, out CachedSnippets cachedSnippets)
        {
            return directoryToSnippets.TryRemove(directory, out cachedSnippets);
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        [Time]
        public Task<CachedSnippets> FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            CachedSnippets cachedSnippets;
            if (!directoryToSnippets.TryGetValue(directory, out cachedSnippets))
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            if (cachedSnippets.Ticks != lastDirectoryWrite)
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            return Task.FromResult(cachedSnippets);
        }

        async Task<CachedSnippets> UpdateCache(string directory, long lastDirectoryWrite)
        {
            var readSnippets = await snippetExtractor.FromDirectory(directory)
                .ConfigureAwait(false);
            var snippetGroups = SnippetGrouper.Group(readSnippets.Snippets, convertSnippetPackageGroupToList);
            var cachedSnippets = new CachedSnippets(
                ticks: lastDirectoryWrite,
                readingErrors: readSnippets.GetSnippetsInError().ToList(),
                groupingErrors: snippetGroups.Errors,
                snippetGroups: snippetGroups.Groups);
            return directoryToSnippets[directory] = cachedSnippets;
        }

    }
}