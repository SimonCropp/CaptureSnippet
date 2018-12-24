using System.IO;
using CaptureSnippets;

class Usage
{
    void Basic()
    {
        #region usageSimple
        // setup version convention and extract snippets from files
        var directorySnippetExtractor = new DirectorySnippetExtractor(
            directoryFilter: x => true,
            fileFilter: s => s.EndsWith(".vm") || s.EndsWith(".cs"));
        var snippets = directorySnippetExtractor.ReadSnippets(@"C:\path");

        // Merge with some markdown text
        var markdownProcessor = new MarkdownProcessor(snippets.Lookup, SimpleSnippetMarkdownHandling.AppendGroup);

        //In this case the text will be extracted from a file path
        ProcessResult result;
        using (var reader = File.OpenText(@"C:\path\myInputMarkdownFile.md"))
        using (var writer = File.CreateText(@"C:\path\myOutputMarkdownFile.md"))
        {
            result = markdownProcessor.Apply(reader, writer);
        }

        // List of all snippets that the markdown file expected but did not exist in the input snippets
        var missingSnippets = result.MissingSnippets;

        // List of all snippets that the markdown file used
        var usedSnippets = result.UsedSnippets;
        #endregion
    }
}