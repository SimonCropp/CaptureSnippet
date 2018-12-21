using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CaptureSnippets
{
    public class DirectorySnippetExtractor
    {
        DirectoryFilter directoryFilter;
        FileFilter fileFilter;

        public DirectorySnippetExtractor(
            DirectoryFilter directoryFilter,
            FileFilter fileFilter)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            this.directoryFilter = directoryFilter;
            this.fileFilter = fileFilter;
        }

        public ReadSnippets ReadSnippets(string directory)
        {
            var snippetExtractor = FileSnippetExtractor.BuildShared();
            var packages = ReadSnippets(directory, snippetExtractor).ToList();
            return new ReadSnippets(directory, packages);
        }

        void FindFiles(string directoryPath, List<string> files)
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath)
                .Where(s => fileFilter(s)))
            {
                files.Add(file);
            }
            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(s => directoryFilter(s)))
            {
                FindFiles(subDirectory, files);
            }
        }

        IEnumerable<Snippet> ReadSnippets(string directory, FileSnippetExtractor snippetExtractor)
        {
            var files = new List<string>();
            FindFiles(directory, files);
            return files
                .SelectMany(file =>
                {
                    using (var reader = File.OpenText(file))
                    {
                        return snippetExtractor.AppendFromReader(reader, file).ToList();
                    }
                });
        }
    }
}