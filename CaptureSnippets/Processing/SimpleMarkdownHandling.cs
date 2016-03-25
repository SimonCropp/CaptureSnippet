using System.IO;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace CaptureSnippets
{

    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleMarkdownHandling
    {

        /// <summary>
        /// Method that can be override to control how an individual <see cref="SnippetGroup"/> is rendered.
        /// </summary>
        public static async Task AppendGroup(SnippetGroup snippetGroup, TextWriter writer)
        {
            Guard.AgainstNull(snippetGroup, "snippetGroup");
            Guard.AgainstNull(writer, "writer");

            var language = snippetGroup.Language;
            foreach (var packageGroup in snippetGroup)
            {
                await AppendPackageGroup(writer, packageGroup, language);
            }
        }

        static async Task AppendPackageGroup(TextWriter writer, PackageGroup packageGroup, string language)
        {
            if (packageGroup.Package != null)
            {
                var message = $"### Package: '{packageGroup.Package}'";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            foreach (var version in packageGroup.Versions)
            {
                await AppendVersionGroup(writer, version, language)
                    .ConfigureAwait(false);
            }
        }

        static async Task AppendVersionGroup(TextWriter writer, VersionGroup versionGroup, string language)
        {
            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = $"#### Version: '{versionGroup.Version.ToFriendlyString()}'";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            var format = $@"```{language}
{versionGroup.Value}
```";
            await writer.WriteLineAsync(format)
                .ConfigureAwait(false);
        }

    }
}