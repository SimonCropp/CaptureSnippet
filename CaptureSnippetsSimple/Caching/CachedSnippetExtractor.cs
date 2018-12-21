using System.Collections.Concurrent;

namespace CaptureSnippets
{
    /// <summary>
    /// Provides a higher level abstraction over snippets parsing
    /// </summary>
    public class CachedSnippetExtractor
    {
        DirectorySnippetExtractor extractor;
        ConcurrentDictionary<string, CachedComponents> componentCache = new ConcurrentDictionary<string, CachedComponents>();
        ConcurrentDictionary<string, CachedPackages> packageCache = new ConcurrentDictionary<string, CachedPackages>();
        ConcurrentDictionary<string, CachedSnippets> snippetCache = new ConcurrentDictionary<string, CachedSnippets>();

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
        public bool TryRemoveDirectory(string directory, out CachedComponents cached)
        {
            return componentCache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        public bool TryRemoveDirectory(string directory, out CachedPackages cached)
        {
            return packageCache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        public bool TryRemoveDirectory(string directory, out CachedSnippets cached)
        {
            return snippetCache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        public CachedPackages PackagesFromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            if (!packageCache.TryGetValue(directory, out var cached))
            {
                return UpdatePackages(directory, lastDirectoryWrite);
            }
            if (cached.Ticks != lastDirectoryWrite)
            {
                return UpdatePackages(directory, lastDirectoryWrite);
            }
            return cached;
        }

        CachedPackages UpdatePackages(string directory, long lastDirectoryWrite)
        {
            var packages = extractor.ReadPackages(directory);
            var cachedSnippets = new CachedPackages(
                ticks: lastDirectoryWrite,
                readPackages: packages);
            return packageCache[directory] = cachedSnippets;
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        public CachedSnippets SnippetsFromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            if (!snippetCache.TryGetValue(directory, out var cached))
            {
                return UpdateSnippets(directory, lastDirectoryWrite);
            }
            if (cached.Ticks != lastDirectoryWrite)
            {
                return UpdateSnippets(directory, lastDirectoryWrite);
            }
            return cached;
        }

        CachedSnippets UpdateSnippets(string directory, long lastDirectoryWrite)
        {
            var snippets = extractor.ReadSnippets(directory);
            var cachedSnippets = new CachedSnippets(
                ticks: lastDirectoryWrite,
                readSnippets: snippets);
            return snippetCache[directory] = cachedSnippets;
        }

        /// <summary>
        /// Extract all <see cref="ReadComponents"/> from a given directory.
        /// </summary>
        public CachedComponents ComponentsFromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            if (!componentCache.TryGetValue(directory, out var cached))
            {
                return UpdateComponent(directory, lastDirectoryWrite);
            }
            if (cached.Ticks != lastDirectoryWrite)
            {
                return UpdateComponent(directory, lastDirectoryWrite);
            }
            return cached;
        }

        CachedComponents UpdateComponent(string directory, long lastDirectoryWrite)
        {
            var components = extractor.ReadComponents(directory);
            var cachedSnippets = new CachedComponents(
                ticks: lastDirectoryWrite,
                components: components);
            return componentCache[directory] = cachedSnippets;
        }
    }
}