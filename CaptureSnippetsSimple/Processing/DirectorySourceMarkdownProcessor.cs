using System;
using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    public static class DirectorySourceMarkdownProcessor
    {
        public static void Run(string targetDirectory, IEnumerable<string> snippetFiles)
        {
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
            Guard.AgainstNull(snippetFiles, nameof(snippetFiles));
            var sourceMdFileFinder = new FileFinder(path => true, IsSourceMd);

            var snippets = FileSnippetExtractor.Read(snippetFiles);
            var markdownProcessor = new MarkdownProcessor(snippets.ToDictionary(), SimpleSnippetMarkdownHandling.AppendGroup);
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