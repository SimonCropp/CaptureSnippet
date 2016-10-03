using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public class DirectorySnippetExtractor
    {
        DirectoryFilter directoryFilter;
        FileFilter fileFilter;
        GetPackageOrderForComponent packageOrder;

        public DirectorySnippetExtractor(DirectoryFilter directoryFilter, FileFilter fileFilter, GetPackageOrderForComponent packageOrder)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            this.directoryFilter = directoryFilter;
            this.fileFilter = fileFilter;
            this.packageOrder = packageOrder;
        }

        public ReadComponents ReadComponents(string directory)
        {
            var components = EnumerateComponents(directory).ToList();
            var shared = GetShared(directory);
            return new ReadComponents(components, shared);
        }

        List<Snippet> GetShared(string directory)
        {
            var sharedDirectory = Path.Combine(directory, "Shared");
            if (Directory.Exists(sharedDirectory))
            {

                var snippetExtractor = FileSnippetExtractor.BuildShared();
                return ReadSnippets(sharedDirectory, snippetExtractor).ToList();
            }
            return new List<Snippet>();
        }

        public ReadPackages ReadPackages(string directory)
        {
            var packages = EnumeratePackages(directory).ToList();

            var shared = GetShared(directory);
            return new ReadPackages(packages, shared);
        }

        IEnumerable<Package> EnumeratePackages(string directory)
        {
            var packageDirectories = Directory.EnumerateDirectories(directory, "*_*")
                .Where(s => s != "Shared" && directoryFilter(s));

            var lookup = new Dictionary<string, List<VersionGroup>>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageAndVersionDirectory in packageDirectories)
            {
                var name = Path.GetFileName(packageAndVersionDirectory);
                var index = name.IndexOf('_');
                if (index < 1)
                {
                    throw new Exception($"Expected the directroy name '{name}' to be split by a \'_\'.");
                }
                var packagePart = name.Substring(0, index);
                var versionPart = name.Substring(index + 1, name.Length - index - 1);
                List<VersionGroup> versions;
                if (!lookup.TryGetValue(packagePart, out versions))
                {
                    lookup[packagePart] = versions = new List<VersionGroup>();
                }

                var version = VersionRangeParser.ParseVersion(versionPart);
                versions.Add(ReadVersion(packageAndVersionDirectory, version, packagePart));
            }
            foreach (var kv in lookup)
            {
                yield return new Package(kv.Key, kv.Value);
            }
        }

        internal IEnumerable<Component> EnumerateComponents(string directory)
        {
            return Directory.EnumerateDirectories(directory)
                .Where(s => s != "Shared" && directoryFilter(s))
                .Select(ReadComponent);
        }

        Component ReadComponent(string componentDirectory)
        {
            var packages = EnumeratePackages(componentDirectory);
            var name = Path.GetDirectoryName(componentDirectory);
            var shared = GetShared(componentDirectory);
            return new Component(
                identifier: name,
                packages: GetOrderedPackages(name, packages).ToList(),
                shared: shared);
        }


        IEnumerable<Package> GetOrderedPackages(string component, IEnumerable<Package> package)
        {
            if (packageOrder == null)
            {
                return package.OrderBy(_ => _.Identifier);
            }
            List<string> result;
            try
            {
                result = packageOrder(component).ToList();
            }
            catch (Exception exception)
            {
                var errorMessage = $"Error getting package order. Component='{component}'.";
                throw new Exception(errorMessage, exception);
            }

            return package.OrderBy(_ =>
            {
                try
                {
                    return result.IndexOf(_.Identifier);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Error getting package index. Component='{component}', Package='{_.Identifier}'.";
                    throw new Exception(errorMessage, exception);
                }
            });
        }

        VersionGroup ReadVersion(string versionDirectory, VersionRange version, string package)
        {
            var snippetExtractor = FileSnippetExtractor.Build(version, package);
            return new VersionGroup(
                version: version,
                snippets: ReadSnippets(versionDirectory, snippetExtractor).ToList());
        }

        IEnumerable<Snippet> ReadSnippets(string directory, FileSnippetExtractor snippetExtractor)
        {
            return Directory.EnumerateFiles(directory)
                .Where(s => fileFilter(s))
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