using System.Collections.Concurrent;
using System.Linq;
using MethodTimer;

namespace CaptureSnippets
{
    /// <summary>
    /// Provides a higher level abstraction over include parsing
    /// </summary>
    public class CachedIncludeExtractor
    {
        IncludeExtractor extractor;
        ConcurrentDictionary<string, CachedIncludes> cache = new ConcurrentDictionary<string, CachedIncludes>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extractPathData">The convention that is passed to <see cref="DirectorySnippetExtractor"/>.</param>
        public CachedIncludeExtractor(ExtractIncludeData extractIncludeData, ExtractPathData extractPathData, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(extractPathData, "extractPathData");
            Guard.AgainstNull(extractIncludeData, "extractIncludeData");
            extractor = new IncludeExtractor(extractIncludeData, extractPathData, translatePackage);
        }

        /// <summary>
        /// Attempts to remove and return the the cached value for <paramref name="directory"/> from the underlying <see cref="ConcurrentDictionary{TKey,TValue}"/> using <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove"/>.
        /// </summary>
        [Time]
        public bool TryRemoveDirectory(string directory, out CachedIncludes cached)
        {
            return cache.TryRemove(directory, out cached);
        }

        /// <summary>
        /// Extract all snippets from a given directory.
        /// </summary>
        [Time]
        public CachedIncludes FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(directory);

            CachedIncludes cached;
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

        CachedIncludes UpdateCache(string directory, long lastDirectoryWrite)
        {
            var read = extractor.FromDirectory(directory);
            var groups = IncludeGrouper.Group(read.Includes);
            var cached = new CachedIncludes(
                ticks: lastDirectoryWrite,
                readingErrors: read.GetIncludesInError().ToList(),
                groupingErrors: groups.Errors,
                includeGroups: groups.Groups);
            return cache[directory] = cached;
        }

    }
}