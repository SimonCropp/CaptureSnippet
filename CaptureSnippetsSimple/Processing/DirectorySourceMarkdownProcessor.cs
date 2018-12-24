using System;
using System.IO;

namespace CaptureSnippets
{
    public class DirectorySourceMarkdownProcessor
    {
        public static void Run(DirectoryFilter snippetDirectoryFilter, FileFilter snippetFileFilter)
        {
            if (!GitRepoDirectoryFinder.TryFind(out var rootDirectory))
            {
                throw new Exception("Could not find root git directory");
            }
            Guard.AgainstNull(snippetDirectoryFilter, nameof(snippetDirectoryFilter));
            Guard.AgainstNull(snippetFileFilter, nameof(snippetFileFilter));
            FileFilter sourceMdFileFilter = x => x.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
            var sourceMdFileFinder = new FileFinder(path => true, sourceMdFileFilter);
            var extractor = new DirectorySnippetExtractor(snippetDirectoryFilter, snippetFileFilter);
            var readSnippets = extractor.ReadSnippets(rootDirectory);

            var snippets = readSnippets.Snippets.ToDictionary();
            var markdownProcessor = new MarkdownProcessor(snippets,SimpleSnippetMarkdownHandling.AppendGroup);
            foreach (var sourceFile in sourceMdFileFinder.FindFiles(rootDirectory))
            {
                var target = sourceFile.Replace(".source.md",".md");
                var contents = markdownProcessor.Apply(File.ReadAllText(sourceFile));
                File.WriteAllText(target, contents);
            }
        }
    }
}