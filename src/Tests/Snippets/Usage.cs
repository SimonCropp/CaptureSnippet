using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;
using NuGet.Versioning;

class Usage
{
    void ReadingFiles()
    {
        #region ReadingFiles

        var files = Directory.EnumerateFiles(@"C:\path", "*.cs", SearchOption.AllDirectories);

        var snippetExtractor = FileSnippetExtractor.Build(
            fileVersion: VersionRange.Parse("[1.1,2.0)"),
            package: "ThePackageName",
            isCurrent: true);
        var snippets = snippetExtractor.Read(files);

        #endregion
    }

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
        var components = snippetExtractor.ReadComponents(@"C:\path");
        var component1 = components.GetComponent("Component1");
        var packagesForComponent1 = component1.Packages;
        var snippetsForComponent1 = component1.Snippets;

        var packages = snippetExtractor.ReadPackages(@"C:\path");
        var package1 = components.GetComponent("Package1");
        var snippetsForPackage1 = package1.Snippets;

        // The below snippets could also be accessed via
        //  * packages.Snippets
        //  * components.AllSnippets
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