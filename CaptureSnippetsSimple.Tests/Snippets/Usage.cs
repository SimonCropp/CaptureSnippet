﻿using System.IO;
using CaptureSnippets;

class Usage
{
    void ReadingDirectory()
    {
        #region ReadingDirectorySimple

        // extract snippets from files
        var snippetExtractor = new DirectorySnippetExtractor(
            // all directories except bin and obj
            directoryFilter: dirPath => !dirPath.EndsWith("bin") && !dirPath.EndsWith("obj"),
            // all vm and cs files
            fileFilter: filePath => filePath.EndsWith(".vm") || filePath.EndsWith(".cs"));
        var snippets = snippetExtractor.ReadSnippets(@"C:\path");

        #endregion
    }

    void Basic()
    {
        #region markdownProcessingSimple

        // setup version convention and extract snippets from files
        var snippetExtractor = new DirectorySnippetExtractor(
            directoryFilter: x => true,
            fileFilter: s => s.EndsWith(".vm") || s.EndsWith(".cs"));
        var snippets = snippetExtractor.ReadSnippets(@"C:\path");

        // Merge with some markdown text
        var markdownProcessor = new MarkdownProcessor(snippets, SimpleSnippetMarkdownHandling.AppendGroup);

        using (var reader = File.OpenText(@"C:\path\inputMarkdownFile.md"))
        using (var writer = File.CreateText(@"C:\path\outputMarkdownFile.md"))
        {
            var result = markdownProcessor.Apply(reader, writer);
            // snippets that the markdown file expected but did not exist in the input snippets
            var missingSnippets = result.MissingSnippets;

            // snippets that the markdown file used
            var usedSnippets = result.UsedSnippets;
        }

        #endregion
    }
}