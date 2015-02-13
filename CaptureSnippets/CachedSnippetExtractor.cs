using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        Func<string, bool> includeDirectory;
        Func<string, bool> includeFile;
        SnippetExtractor snippetExtractor;

        ConcurrentDictionary<string, CachedSnippets> directoryToSnippets = new ConcurrentDictionary<string, CachedSnippets>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="versionFromFilePathExtractor">The version convention that is passed to <see cref="SnippetExtractor"/>.</param>
        /// <param name="includeDirectory">Directories to include.</param>
        /// <param name="includeFile">Files to include.</param>
        public CachedSnippetExtractor(Func<string, Version> versionFromFilePathExtractor, Func<string,bool> includeDirectory, Func<string,bool> includeFile)
        {
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
            snippetExtractor = new SnippetExtractor(versionFromFilePathExtractor);
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
        public async Task<CachedSnippets> FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var includeDirectories = new List<string>();
            GetDirectoriesToInclude(directory, includeDirectories);
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(includeDirectories);
           
            CachedSnippets cachedSnippets;
            if (!directoryToSnippets.TryGetValue(directory, out cachedSnippets))
            {
                return await UpdateCache(directory, includeDirectories, lastDirectoryWrite).ConfigureAwait(false);
            }
            if (cachedSnippets.Ticks != lastDirectoryWrite)
            {
                return await UpdateCache(directory, includeDirectories, lastDirectoryWrite).ConfigureAwait(false);
            }
            return cachedSnippets;
        }

        async Task<CachedSnippets> UpdateCache(string directory, List<string> includeDirectories, long lastDirectoryWrite)
        {
            var readSnippets = await snippetExtractor.FromFiles(GetFilesToInclude(includeDirectories)).ConfigureAwait(false);
            var snippetGroups = SnippetGrouper.Group(readSnippets.Snippets).ToList();
            return directoryToSnippets[directory] = new CachedSnippets
                                             {
                                                 Ticks = lastDirectoryWrite,
                                                 Errors = readSnippets.Errors,
                                                 SnippetGroups = snippetGroups
                                             };
        }

        void GetDirectoriesToInclude(string directory, List<string> includedDirectories)
        {
            if (!includeDirectory(directory))
            {
                return;
            }
            includedDirectories.Add(directory);
            foreach (var child in Directory.EnumerateDirectories(directory, "*"))
            {
                GetDirectoriesToInclude(child, includedDirectories);
            }
        }

        IEnumerable<string> GetFilesToInclude(List<string> includedDirectories)
        {
            return from directory in includedDirectories 
                   from file in Directory.EnumerateFiles(directory, "*") 
                   where includeFile(Path.GetFileName(file)) select file;
        }
    }
}