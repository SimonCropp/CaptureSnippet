using System.IO;
using NuGet.Versioning;

namespace CaptureSnippets
{

    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleSnippetMarkdownHandling
    {

        public static void AppendGroup(SnippetGroup group, TextWriter writer)
        {
            Guard.AgainstNull(group, nameof(group));
            Guard.AgainstNull(writer, nameof(writer));

            var message = $"### Key: '{group.Key}'";
            writer.WriteLine(message);
            var language = group.Language;
            foreach (var package in group)
            {
                AppendPackageGroup(writer, package, language);
            }
        }

        static void AppendPackageGroup(TextWriter writer, PackageGroup packageGroup, string language)
        {
            if (packageGroup.Package != Package.Undefined)
            {
                var message = $"### Package: '{packageGroup.Package}'";
                writer.WriteLine(message);
            }
            foreach (var version in packageGroup.Versions)
            {
                AppendVersionGroup(writer, version, language);
            }
        }

        static void AppendVersionGroup(TextWriter writer, VersionGroup versionGroup, string language)
        {
            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = $"#### Version: '{versionGroup.Version.ToFriendlyString()}'";
                writer.WriteLine(message);
            }
            var format = $@"```{language}
{versionGroup.Value}
```";
            writer.WriteLine(format);
        }

    }
}