using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;

class FileFinder
{
    DirectoryFilter directoryFilter;
    FileFilter fileFilter;

    public FileFinder(DirectoryFilter directoryFilter, FileFilter fileFilter)
    {
        this.directoryFilter = directoryFilter;
        this.fileFilter = fileFilter;
    }

    public bool IncludeDirectory(string directoryPath)
    {
        return directoryFilter(directoryPath) && !directoryPath.EndsWith(".git");
    }

    public void FindFiles(string directoryPath, List<string> files)
    {
        foreach (var file in Directory.EnumerateFiles(directoryPath)
            .Where(s => fileFilter(s)))
        {
            files.Add(file);
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
            .Where(IncludeDirectory))
        {
            FindFiles(subDirectory, files);
        }
    }
}