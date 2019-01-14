using System;
using CaptureSnippets;

class Program
{
    static void Main()
    {
        var currentDirectory = Environment.CurrentDirectory;
        DirectorySourceMarkdownProcessor.Run(currentDirectory);
    }
}