using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CaptureSnippets
{
    public static class GitHubMarkdownProcessor
    {
        static Action<string> log;

        static GitHubMarkdownProcessor()
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

        public static void RunForFilePath([CallerFilePath] string sourceFilePath = "")
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var root = GitRepoDirectoryFinder.FindForFilePath(sourceFilePath);
            Run(root);
        }

        public static void Run(string targetDirectory)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            var finder = new FileFinder();
            var findFiles = finder.FindFiles(targetDirectory);
            Run(targetDirectory, findFiles);
        }

        internal static void Run(string targetDirectory, List<string> snippetSourceFiles)
        {
            log($"Searching {snippetSourceFiles.Count} files for snippets");
            var finder = new FileFinder(path => true, IsSourceMd);
            var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
            log($"Found {snippets.Count} snippets");
            var handling = new GitHubSnippetMarkdownHandling(targetDirectory);
            var processor = new MarkdownProcessor(snippets, handling.AppendGroup);
            var sourceFiles = finder.FindFiles(targetDirectory);
            log($"Found {sourceFiles.Count} .source.md files");
            foreach (var sourceFile in sourceFiles)
            {
                ProcessFile(sourceFile, processor);
            }
        }

        static void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor)
        {
            log($"Processing {sourceFile}");
            var target = sourceFile.Replace(".source.md", ".md");
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            using (var reader = File.OpenText(sourceFile))
            using (var writer = File.CreateText(target))
            {
                var processResult = markdownProcessor.Apply(reader, writer);
                var missing = processResult.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
            }
        }

        static bool IsSourceMd(string path)
        {
            return path.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
        }
    }
}