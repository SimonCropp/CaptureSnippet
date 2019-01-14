using System;
using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    public static class DirectorySourceMarkdownProcessor
    {
        public static void Run(string targetDirectory)
        {
            var snippetFileFinder = new FileFinder();
            var findFiles = snippetFileFinder.FindFiles(targetDirectory);
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
            Run(targetDirectory, findFiles);
        }

        public static void Run(string targetDirectory, IEnumerable<string> findFiles)
        {
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
            Guard.AgainstNull(findFiles, nameof(findFiles));
            var sourceMdFileFinder = new FileFinder(path => true, IsSourceMd);
            var snippets = FileSnippetExtractor.Read(findFiles);
            var markdownProcessor = new MarkdownProcessor(snippets, SimpleSnippetMarkdownHandling.AppendGroup);
            foreach (var sourceFile in sourceMdFileFinder.FindFiles(targetDirectory))
            {
                var target = sourceFile.Replace(".source.md", ".md");
                var contents = markdownProcessor.Apply(File.ReadAllText(sourceFile));
                File.WriteAllText(target, contents);
            }
        }

        static bool IsSourceMd(string path)
        {
            return path.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
        }
    }
}