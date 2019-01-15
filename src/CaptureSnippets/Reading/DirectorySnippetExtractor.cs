using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    public class DirectorySnippetExtractor
    {
        FileFinder fileFinder;
        GetPackageOrderForComponent packageOrder;
        TranslatePackage translatePackage;

        internal DirectorySnippetExtractor(
            GetPackageOrderForComponent packageOrder = null,
            TranslatePackage translatePackage = null) :
            this(path => true, path => true, packageOrder, translatePackage)
        {
        }

        public DirectorySnippetExtractor(
            DirectoryFilter directoryFilter,
            FileFilter fileFilter,
            GetPackageOrderForComponent packageOrder = null,
            TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(directoryFilter, nameof(directoryFilter));
            Guard.AgainstNull(fileFilter, nameof(fileFilter));
            fileFinder = new FileFinder(directoryFilter, fileFilter);
            if (packageOrder == null)
            {
                this.packageOrder = x => Enumerable.Empty<string>();
            }
            else
            {
                this.packageOrder = packageOrder;
            }

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

            var allPath = Path.Combine(directory, Path.GetFileName(directory) + "_All");
            if (Directory.Exists(allPath))
            {
                var snippetExtractor = FileSnippetExtractor.BuildShared();
                return ReadSnippets(allPath, snippetExtractor).ToList();
            }

            return new List<Snippet>();
        }

        public ReadPackages ReadPackages(string directory)
        {
            var componentShared = GetShared(directory);
            var packages = EnumeratePackages(directory, null, new List<Snippet>(), componentShared).ToList();
            return new ReadPackages(packages, directory, componentShared);
        }

        public ReadSnippets ReadSnippets(string directory)
        {
            var snippetExtractor = FileSnippetExtractor.BuildShared();
            var packages = ReadSnippets(directory, snippetExtractor).ToList();
            return new ReadSnippets(directory, packages);
        }

        class PackageVersionCurrent
        {
            public NuGetVersion Version;
            public string Package;
            public string PackageAlias;
            public string Directory;
            public bool IsCurrent;
        }

        IEnumerable<PackageVersionCurrent> GetOrderedPackages(string component, IEnumerable<PackageVersionCurrent> package)
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
                var errorMessage = $"Error getting package order from supplied GetPackageOrderForComponent. Component='{component}'.";
                throw new Exception(errorMessage, exception);
            }

            return package.OrderBy(_ => result.IndexOf(_.Package));
        }

        IEnumerable<Package> EnumeratePackages(string directory, string component, List<Snippet> globalShared, List<Snippet> componentShared)
        {
            var packageDirectories = Directory.EnumerateDirectories(directory, "*_*")
                .Where(s => !IsShared(s) &&
                            fileFinder.IncludeDirectory(s));

            var packageVersionList = new List<PackageVersionCurrent>();
            foreach (var packageAndVersionDirectory in packageDirectories)
            {
                var name = Path.GetFileName(packageAndVersionDirectory);
                var index = name.IndexOf('_');
                if (index < 1)
                {
                    throw new SnippetReadingException($"Expected the directory name '{name}' to be split by a '_'.");
                }

                var packageAlias = name.Substring(0, index);
                var package = translatePackage(packageAlias);
                var versionPart = name.Substring(index + 1, name.Length - index - 1);

                var pretext = PreTextReader.GetPretext(packageAndVersionDirectory);
                var version = VersionRangeParser.ParseVersion(versionPart, pretext);
                var item = new PackageVersionCurrent
                {
                    Version = version,
                    Package = package,
                    PackageAlias = packageAlias,
                    Directory = packageAndVersionDirectory,
                };
                packageVersionList.Add(item);
            }

            if (!packageVersionList.Any())
            {
                yield break;
            }

            packageVersionList = packageVersionList
                .OrderByDescending(x => x.Version)
                .ToList();
            packageVersionList = GetOrderedPackages(component, packageVersionList).ToList();

            SetCurrent(packageVersionList);
            foreach (var group in packageVersionList.GroupBy(
                keySelector: _ => _.Package,
                comparer: StringComparer.InvariantCultureIgnoreCase))
            {
                var versions = new List<VersionGroup>();
                NuGetVersion previous = null;
                foreach (var packageAndVersion in group)
                {
                    VersionRange versionRange;
                    var minVersion = packageAndVersion.Version;

                    if (previous == null)
                    {
                        versionRange = new VersionRange(
                            minVersion: minVersion,
                            includeMinVersion: true,
                            maxVersion: new NuGetVersion(minVersion.Major + 1, 0, 0),
                            includeMaxVersion: false
                        );
                    }
                    else
                    {
                        versionRange = new VersionRange(
                            minVersion: minVersion,
                            includeMinVersion: true,
                            maxVersion: new NuGetVersion(previous.Major, previous.Minor, previous.Patch),
                            includeMaxVersion: false
                        );
                    }

                    previous = minVersion;

                    var versionGroup = ReadVersion(
                        versionDirectory: packageAndVersion.Directory,
                        version: versionRange,
                        package: packageAndVersion.Package,
                        packageAlias: packageAndVersion.PackageAlias,
                        isCurrent: packageAndVersion.IsCurrent,
                        componentShared: componentShared,
                        globalShared: globalShared);
                    versions.Add(versionGroup);
                }

                yield return new Package(group.Key, versions);
            }
        }

        static bool IsShared(string directory)
        {
            var directorySuffix = Path.GetFileName(directory);
            return string.Equals(directorySuffix, "Shared", StringComparison.OrdinalIgnoreCase) ||
                   directorySuffix.EndsWith("_All", StringComparison.OrdinalIgnoreCase);
        }

        static void SetCurrent(List<PackageVersionCurrent> packageVersionList)
        {
            var firstStable = packageVersionList.FirstOrDefault(_ => !_.Version.IsPrerelease);
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
                .Where(s => !IsShared(s) && fileFinder.IncludeDirectory(s))
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

        VersionGroup ReadVersion(
            string versionDirectory,
            VersionRange version,
            string package,
            string packageAlias,
            bool isCurrent,
            IReadOnlyList<Snippet> componentShared,
            List<Snippet> globalShared)
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
                package: package,
                packageAlias: packageAlias);
        }

        IEnumerable<Snippet> ReadSnippets(string directory, FileSnippetExtractor snippetExtractor)
        {
            return fileFinder.FindFiles(directory)
                .SelectMany(file =>
                {
                    using (var reader = File.OpenText(file))
                    {
                        return snippetExtractor.Read(reader, file).ToList();
                    }
                });
        }
    }
}