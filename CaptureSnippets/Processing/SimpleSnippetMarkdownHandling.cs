using System.IO;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace CaptureSnippets
{

    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleSnippetMarkdownHandling
    {

        public static async Task AppendGroup(SnippetGroup group, TextWriter writer)
        {
            Guard.AgainstNull(group, "group");
            Guard.AgainstNull(writer, "writer");

            var language = group.Language;
            foreach (var package in group)
            {
                await AppendPackageGroup(writer, package, language);
            }
        }

        static async Task AppendPackageGroup(TextWriter writer, SnippetPackageGroup packageGroup, string language)
        {
            if (packageGroup.Package != Package.Undefined)
            {
                var message = $"### Package: '{packageGroup.Package}'";
                await writer.WriteLineAsync(message);
            }
            foreach (var version in packageGroup.Versions)
            {
                await AppendVersionGroup(writer, version, language);
            }
        }

        static async Task AppendVersionGroup(TextWriter writer, SnippetVersionGroup versionGroup, string language)
        {
            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = $"#### Version: '{versionGroup.Version.ToFriendlyString()}'";
                await writer.WriteLineAsync(message);
            }
            var format = $@"```{language}
{versionGroup.Value}
```";
            await writer.WriteLineAsync(format);
        }

    }
}