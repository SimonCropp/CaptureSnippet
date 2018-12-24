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
            FileFilter sourceMdFileFilter = x => x.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
            var sourceMdFileFinder = new FileFinder(path => true, sourceMdFileFilter);
            var extractor = new DirectorySnippetExtractor(snippetDirectoryFilter, snippetFileFilter);
            var readSnippets = extractor.ReadSnippets(directory);

            var snippets = readSnippets.Snippets.ToDictionary();
            var markdownProcessor = new MarkdownProcessor(snippets, SimpleSnippetMarkdownHandling.AppendGroup);
            foreach (var sourceFile in sourceMdFileFinder.FindFiles(directory))
            {
                var target = sourceFile.Replace(".source.md", ".md");
                var contents = markdownProcessor.Apply(File.ReadAllText(sourceFile));
                File.WriteAllText(target, contents);
            }
        }
    }
}