using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using MethodTimer;

namespace CaptureSnippets
{

    class CachedSnippets
    {
        public ReadSnippets Snippets;
        public long Ticks;
    }
    public class CachedSnippetExtractor
    {
        Func<string, bool> includeDirectory;
        Func<string, bool> includeFile;
        SnippetExtractor snippetExtractor;

        ConcurrentDictionary<string, CachedSnippets> directoryToSnippets = new ConcurrentDictionary<string, CachedSnippets>();

        public CachedSnippetExtractor(Func<string, Version> versionFromFilePathExtractor, Func<string,bool> includeDirectory, Func<string,bool> includeFile)
        {
            this.includeDirectory = includeDirectory;
            this.includeFile = includeFile;
            snippetExtractor = new SnippetExtractor(versionFromFilePathExtractor);
        }

        [Time]
        public ReadSnippets FromDirectory(string directory)
        {
            directory = directory.ToLower();
            var includeDirectories = new List<string>();
            GetDirectoriesToInclude(directory, includeDirectories);
            var lastDirectoryWrite = DirectoryDateFinder.GetLastDirectoryWrite(includeDirectories);
           
            CachedSnippets cachedSnippets;
            if (!directoryToSnippets.TryGetValue(directory, out cachedSnippets))
            {
                return UpdateCache(directory, includeDirectories, lastDirectoryWrite);
            }
            if (cachedSnippets.Ticks != lastDirectoryWrite)
            {
                return UpdateCache(directory, includeDirectories, lastDirectoryWrite);
            }
            return cachedSnippets.Snippets;
        }

        ReadSnippets UpdateCache(string directory, List<string> includeDirectories, long lastDirectoryWrite)
        {
            var readSnippets = snippetExtractor.FromFiles(GetFilesToInclude(includeDirectories));
            directoryToSnippets[directory] = new CachedSnippets
                                             {
                                                 Ticks = lastDirectoryWrite,
                                                 Snippets = readSnippets
                                             };
            return readSnippets;
        }

        void GetDirectoriesToInclude(string directory, List<string> includedDirectories)
        {
            foreach (var child in Directory.EnumerateDirectories(directory, "*"))
            {
                if (!includeDirectory(child))
                {
                    continue;
                }
                includedDirectories.Add(child); 
                GetDirectoriesToInclude(child,includedDirectories);
            }
        }
        IEnumerable<string> GetFilesToInclude(List<string> includedDirectories)
        {
            foreach (var directory in includedDirectories)
            {
                foreach (var file in Directory.EnumerateFiles(directory, "*"))
                {
                    if (!includeFile(Path.GetFileName(file)))
                    {
                        continue;
                    }
                    yield return file;
                }
            }
        }
    }
}