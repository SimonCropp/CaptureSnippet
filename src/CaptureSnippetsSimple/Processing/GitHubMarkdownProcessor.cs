using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CaptureSnippets
{
    public static class GitHubMarkdownProcessor
    {
        static Action<string> log;

        static  GitHubMarkdownProcessor()
        {
            log = s => { };
        }

        public static Action<string> Log
        {
            get => log;
            set
            {
                Guard.AgainstNull(value, nameof(value));
                log = value;
            }
        }

        public static void Run(string targetDirectory)
        {
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
            var snippetFileFinder = new FileFinder();
            var findFiles = snippetFileFinder.FindFiles(targetDirectory);
            Run(targetDirectory, findFiles);
        }

        internal static void Run(string targetDirectory, List<string> snippetSourceFiles)
        {
            log($"Searching {snippetSourceFiles.Count} files for snippets");
            var sourceMdFileFinder = new FileFinder(path => true, IsSourceMd);
            var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
            log($"Found {snippets.Count} snippets");
            var markdownProcessor = new MarkdownProcessor(snippets, SimpleSnippetMarkdownHandling.AppendGroup);
            var sourceFiles = sourceMdFileFinder.FindFiles(targetDirectory);
            log($"Found {sourceFiles.Count} source.md files");
            foreach (var sourceFile in sourceFiles)
            {
                ProcessFile(sourceFile, markdownProcessor);
            }
        }

        static void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor)
        {
            log($"Processing {sourceFile}");
            var target = sourceFile.Replace(".source.md", ".md");
            var contents = markdownProcessor.Apply(File.ReadAllText(sourceFile));
            File.WriteAllText(target, contents);
        }

        static bool IsSourceMd(string path)
        {
            return path.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
        }
    }
}