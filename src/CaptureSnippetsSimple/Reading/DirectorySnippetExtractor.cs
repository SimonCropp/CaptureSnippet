using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CaptureSnippets
{
    public class DirectorySnippetExtractor
    {
        FileFinder fileFinder;

        public DirectorySnippetExtractor(DirectoryFilter directoryFilter = null, FileFilter fileFilter = null)
        {
            fileFinder = new FileFinder(directoryFilter, fileFilter);
        }

        public ReadSnippets ReadSnippets(string directory)
        {
            var snippets = ReadSnippetsInner(directory).ToList();
            return new ReadSnippets(directory, snippets);
        }

        IEnumerable<Snippet> ReadSnippetsInner(string directory)
        {
            return fileFinder.FindFiles(directory)
                .SelectMany(file =>
                {
                    using (var reader = File.OpenText(file))
                    {
                        return FileSnippetExtractor.Read(reader, file).ToList();
                    }
                });
        }
    }
}