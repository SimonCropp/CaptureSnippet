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
        TranslatePackage translatePackage;

        public DirectorySnippetExtractor(DirectoryFilter directoryFilter, FileFilter fileFilter,
            GetPackageOrderForComponent packageOrder, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            this.directoryFilter = directoryFilter;
            this.fileFilter = fileFilter;
            this.packageOrder = packageOrder;
            if (translatePackage == null)
            {
                this.translatePackage = alias => alias;
            }
            else
            {
                this.translatePackage = translatePackage;
            }
        }

        public ReadComponents ReadComponents(string directory)
        {
            var shared = GetShared(directory);
            var components = EnumerateComponents(directory, shared).ToList();
            return new ReadComponents(components, directory, shared);
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
            var componentShared = GetShared(directory);
            var packages = EnumeratePackages(directory, null, new List<Snippet>(), componentShared).ToList();
            return new ReadPackages(packages, directory, componentShared);
        }

        class PackageVersionCurrent
        {
            public VersionRange Version;
            public string Package;
            public string Directory;
            public bool IsCurrent;
        }


        IEnumerable<PackageVersionCurrent> GetOrderedPackages(string component,
            IEnumerable<PackageVersionCurrent> package)
        {
            if (packageOrder == null)
            {
                return package.OrderBy(_ => _.Package);
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
                    return result.IndexOf(_.Package);
                }
                catch (Exception exception)
                {
                    var errorMessage = $"Error getting package index. Component='{component}', Package='{_.Package}'.";
                    throw new Exception(errorMessage, exception);
                }
            });
        }

        IEnumerable<Package> EnumeratePackages(string directory, string component, List<Snippet> globalShared, List<Snippet> componentShared)
        {
            var packageDirectories = Directory.EnumerateDirectories(directory, "*_*")
                .Where(s => s != "Shared" && directoryFilter(s));

            var packageVersionList = new List<PackageVersionCurrent>();
            foreach (var packageAndVersionDirectory in packageDirectories)
            {
                var name = Path.GetFileName(packageAndVersionDirectory);
                var index = name.IndexOf('_');
                if (index < 1)
                {
                    throw new Exception($"Expected the directory name '{name}' to be split by a '_'.");
                }
                var packagePart = name.Substring(0, index);
                packagePart = translatePackage(packagePart);
                var versionPart = name.Substring(index + 1, name.Length - index - 1);

                var pretext = PreTextReader.GetPretext(packageAndVersionDirectory);
                var version = VersionRangeParser.ParseVersion(versionPart, pretext);
                var item = new PackageVersionCurrent
                {
                    Version = version,
                    Package = packagePart,
                    Directory = packageAndVersionDirectory,
                };
                packageVersionList.Add(item);
            }

            if (!packageVersionList.Any())
            {
                yield break;
            }
            packageVersionList = packageVersionList
                .OrderByDescending(x => x.Version.VersionForCompare())
                .ToList();
            packageVersionList = GetOrderedPackages(component, packageVersionList).ToList();

            SetCurrent(packageVersionList);
            var lookup = new Dictionary<string, List<VersionGroup>>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageAndVersion in packageVersionList)
            {
                List<VersionGroup> versions;
                var package = packageAndVersion.Package;
                if (!lookup.TryGetValue(package, out versions))
                {
                    lookup[package] = versions = new List<VersionGroup>();
                }

                var versionGroup = ReadVersion(packageAndVersion.Directory, packageAndVersion.Version, package,
                    packageAndVersion.IsCurrent, componentShared, globalShared);
                versions.Add(versionGroup);
            }
            foreach (var kv in lookup)
            {
                yield return new Package(kv.Key, kv.Value);
            }
        }

        static void SetCurrent(List<PackageVersionCurrent> packageVersionList)
        {
            var firstStable = packageVersionList.FirstOrDefault(_ => !_.Version.IsPreRelease());
            if (firstStable != null)
            {
                firstStable.IsCurrent = true;
                return;
            }
            packageVersionList.First().IsCurrent = true;
        }

        IEnumerable<Component> EnumerateComponents(string directory, List<Snippet> globalShared)
        {
            return Directory.EnumerateDirectories(directory)
                .Where(s => Path.GetFileName(s) != "Shared" && directoryFilter(s))
                .Select(s => ReadComponent(s, globalShared));
        }


        Component ReadComponent(string componentDirectory, List<Snippet> globalShared)
        {
            var name = Path.GetFileName(componentDirectory);
            var componentShared = GetShared(componentDirectory);
            var packages = EnumeratePackages(componentDirectory, name, globalShared, componentShared).ToList();
            return new Component(
                identifier: name,
                packages: packages,
                directory: componentDirectory,
                shared: globalShared.Concat(componentShared).Distinct().ToList()
            );
        }

        VersionGroup ReadVersion(string versionDirectory, VersionRange version, string package, bool isCurrent,
            IReadOnlyList<Snippet> componentShared, List<Snippet> globalShared)
        {
            var snippetExtractor = FileSnippetExtractor.Build(version, package, isCurrent);
            return new VersionGroup(
                version: version,
                snippets: ReadSnippets(versionDirectory, snippetExtractor)
                    .Concat(componentShared)
                    .Concat(globalShared)
                    .ToList(),
                directory: versionDirectory,
                isCurrent: isCurrent,
                package: package);
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