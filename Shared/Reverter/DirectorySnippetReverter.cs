using System;
using System.IO;

namespace CaptureSnippets
{
    public class DirectorySnippetReverter
    {
        FileFinder fileFinder;

        public DirectorySnippetReverter() : this(path => true, path => true)
        {
        }

        public DirectorySnippetReverter(DirectoryFilter directoryFilter, FileFilter fileFilter)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            FileFilter withMdFileFilter = x =>
            {
                var extension = Path.GetExtension(x);
                return string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase) && fileFilter(x);
            };
            fileFinder = new FileFinder(directoryFilter, withMdFileFilter);
        }

        public void Revert(string directory)
        {
            foreach (var file in fileFinder.FindFiles(directory))
            {
                var contents = FileSnippetReverter.Revert(File.ReadAllText(file));
                File.WriteAllText(file, contents);
            }
        }
    }
}