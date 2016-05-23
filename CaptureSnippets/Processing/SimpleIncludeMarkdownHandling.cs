using System.IO;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace CaptureSnippets
{

    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleIncludeMarkdownHandling
    {

        public static async Task AppendGroup(IncludeGroup group, TextWriter writer)
        {
            Guard.AgainstNull(group, "group");
            Guard.AgainstNull(writer, "writer");

            foreach (var package in group)
            {
                await AppendPackageGroup(writer, package);
            }
        }

        static async Task AppendPackageGroup(TextWriter writer, IncludePackageGroup packageGroup)
        {
            if (packageGroup.Package != Package.None)
            {
                var message = $"### Package: '{packageGroup.Package}'";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            foreach (var version in packageGroup.Versions)
            {
                await AppendVersionGroup(writer, version)
                    .ConfigureAwait(false);
            }
        }

        static async Task AppendVersionGroup(TextWriter writer, IncludeVersionGroup versionGroup)
        {
            if (!versionGroup.Version.Equals(VersionRange.All))
            {
                var message = $"#### Version: '{versionGroup.Version.ToFriendlyString()}'";
                await writer.WriteLineAsync(message)
                    .ConfigureAwait(false);
            }
            await writer.WriteLineAsync(versionGroup.Value)
                .ConfigureAwait(false);
        }

    }
}