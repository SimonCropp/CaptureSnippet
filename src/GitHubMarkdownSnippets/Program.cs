using System;
using System.IO;
using CaptureSnippets;

class Program
{
    static void Main(string[] args)
    {
        var targetDirectory = GetTargetDirectory(args);
        DirectorySourceMarkdownProcessor.Run(targetDirectory);
    }

    static string GetTargetDirectory(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Only one argument (target directory) is supported");
            Environment.Exit(1);
        }

        if (args.Length == 1)
        {
            var targetDirectory = args[0];
            if (Directory.Exists(targetDirectory))
            {
                return targetDirectory;
            }
            Console.WriteLine($"Target directory does not exist: {targetDirectory}");
            Environment.Exit(1);
        }

        var currentDirectory = Environment.CurrentDirectory;
        Console.WriteLine($"Trying to determine root repository path for the current directory: {currentDirectory}");
        if (!GitRepoDirectoryFinder.TryFind(currentDirectory, out var path))
        {
            Console.WriteLine("Could not find a root repository path that contains a .git directory");
            Environment.Exit(1);
        }
        return path;
    }
}