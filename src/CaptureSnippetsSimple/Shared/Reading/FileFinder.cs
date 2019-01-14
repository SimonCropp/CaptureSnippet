using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptureSnippets;

class FileFinder
{
    DirectoryFilter directoryFilter;
    FileFilter fileFilter;

    public FileFinder(DirectoryFilter directoryFilter=null, FileFilter fileFilter=null)
    {
        this.directoryFilter = directoryFilter;
        this.fileFilter = fileFilter;
    }

    public bool IncludeDirectory(string directoryPath)
    {
        var fileName = Path.GetFileName(directoryPath);
        if (fileName.StartsWith("."))
        {
            return false;
        }

        if (Exclusions.ShouldExcludeDirectory(fileName))
        {
            return false;
        }

        if (directoryFilter == null)
        {
            return true;
        }

        return directoryFilter(directoryPath);
    }

    bool IncludeFile(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.StartsWith("."))
        {
            return false;
        }
        var extension = Path.GetExtension(fileName);
        if (extension == string.Empty)
        {
            return false;
        }

        if (Exclusions.ShouldExcludeExtension(extension.Substring(1)))
        {
            return false;
        }

        if (fileFilter == null)
        {
            return true;
        }

        return fileFilter(path);
    }

    public List<string> FindFiles(string directoryPath)
    {
        var files = new List<string>();
        FindFiles(directoryPath, files);
        return files;
    }

    void FindFiles(string directoryPath, List<string> files)
    {
        foreach (var file in Directory.EnumerateFiles(directoryPath)
            .Where(IncludeFile))
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