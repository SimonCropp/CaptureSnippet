using System.IO;

namespace CaptureSnippets
{
    public static class GitRepoDirectoryFinder
    {
        public static bool TryFind(out string path)
        {
            var currentDirectory = AssemblyLocation.CurrentDirectory;
            do
            {
                if (Directory.Exists(Path.Combine(currentDirectory, ".git")))
                {
                    path = currentDirectory;
                    return true;
                }

                var parent = Directory.GetParent(currentDirectory);
                if (parent == null)
                {
                    path = null;
                    return false;
                }

                currentDirectory = parent.FullName;
            } while (true);
        }
    }
}