using System;
using System.IO;
using System.Linq;
using CaptureSnippets;

class Program
{
    static void Main(string[] args)
    {
        var currentDirectory = Environment.CurrentDirectory;
        var files = Directory.EnumerateFiles(currentDirectory, "*.*", SearchOption.AllDirectories)
            .Where(x => new FileInfo(x).Length < 200000);
        DirectorySourceMarkdownProcessor.Run(currentDirectory, files);
    }
}