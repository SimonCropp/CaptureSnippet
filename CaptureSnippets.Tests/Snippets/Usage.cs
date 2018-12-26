using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;

class Usage
{
    void ReadingDirectory()
    {
        #region ReadingDirectory

        IEnumerable<string> PackageOrder(string component)
        {
            if (component == "component1")
            {
                return new List<string>
                {
                    "package1",
                    "package2"
                };
            }

            return Enumerable.Empty<string>();
        }

        string TranslatePackage(string packageAlias)
        {
            if (packageAlias == "shortName")
            {
                return "theFullPackageName";
            }

            return packageAlias;
        }

        // setup version convention and extract snippets from files
        var snippetExtractor = new DirectorySnippetExtractor(
            // all directories except bin and obj
            directoryFilter: dirPath => !dirPath.EndsWith("bin") && !dirPath.EndsWith("obj"),
            // all vm and cs files
            fileFilter: filePath => filePath.EndsWith(".vm") || filePath.EndsWith(".cs"),
            // package order is optional
            packageOrder: PackageOrder,
            // package translation is optional
            translatePackage: TranslatePackage
        );
        var snippets = snippetExtractor.ReadSnippets(@"C:\path");

        #endregion
    }

    void Basic()
    {
        #region markdownProcessing

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