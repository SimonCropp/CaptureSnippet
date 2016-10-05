using System.Collections.Concurrent;
using MethodTimer;

namespace CaptureSnippets
{
    /// <summary>
    /// Provides a higher level abstraction over snippets parsing
    /// </summary>
    public class CachedSnippetExtractor
    {
        DirectorySnippetExtractor extractor;
        ConcurrentDictionary<string, CachedComponents> cache = new ConcurrentDictionary<string, CachedComponents>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="directoryFilter">Directories to include.</param>
        /// <param name="fileFilter">Files to include.</param>
        public CachedSnippetExtractor(DirectoryFilter directoryFilter, FileFilter fileFilter, GetPackageOrderForComponent packageOrder = null, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            extractor = new DirectorySnippetExtractor(directoryFilter, fileFilter, packageOrder, translatePackage);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        [Time]
        public bool TryRemoveDirectory(string directory, out CachedComponents cached)
        {
            return cache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        [Time]
        public CachedComponents FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            CachedComponents cached;
            if (!cache.TryGetValue(directory, out cached))
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            if (cached.Ticks != lastDirectoryWrite)
            {
                return UpdateCache(directory, lastDirectoryWrite);
            }
            return cached;
        }

        CachedComponents UpdateCache(string directory, long lastDirectoryWrite)
        {
            var components = extractor.ReadComponents(directory);
            var cachedSnippets = new CachedComponents(
                ticks: lastDirectoryWrite,
                components: components);
            return cache[directory] = cachedSnippets;
        }

    }
}