using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="ReadSnippet"/>s from a given input.
    /// </summary>
    public class FileSnippetExtractor
    {
        List<ReadSnippet> snippets;

        VersionExtractor versionExtractor;
        PackageExtractor packageExtractor;

        static char[] invalidCharacters = {'“', '”', '—', '`'};
        const string LineEnding = "\r\n";

        /// <summary>
        /// Initialise a new instance of <see cref="FileSnippetExtractor"/>.
        /// </summary>
        /// <param name="snippets">The snippets to append to.</param>
        /// <param name="versionExtractor">How to extract a <see cref="VersionRange"/> from a given path.</param>
        /// <param name="packageExtractor">How to extract a package from a given path. Return null for unknown.</param>
        public FileSnippetExtractor(List<ReadSnippet> snippets, VersionExtractor versionExtractor, PackageExtractor packageExtractor)
        {
            Guard.AgainstNull(snippets, "snippets");
            Guard.AgainstNull(versionExtractor, "versionExtractor");
            Guard.AgainstNull(snippets, "snippets");
            this.snippets = snippets;
            this.versionExtractor = (path, parent) =>
            {
                var version = versionExtractor(path, parent);
                if (version == null)
                {
                    throw new Exception($"VersionExtractor supplied null version for '{path}'.");
                }
                return version;
            };
            this.packageExtractor = (path, parent) =>
            {
                var package = packageExtractor(path, parent);
                if (package.IsEmptyOrWhiteSpace())
                {
                    throw new Exception($"PackageExtractor returned empty string for '{path}'.");
                }
                return package;
            };
        }


        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="parentVersion">The inherited <see cref="VersionRange"/>.</param>
        /// <param name="parentPackage">The inherited package name.</param>
        public async Task AppendFromReader(TextReader textReader, string path, VersionRange parentVersion, string parentPackage)
        {
            Guard.AgainstNull(textReader, "textReader");
            using (var reader = new IndexReader(textReader))
            {
                await GetSnippets(reader, path, parentVersion, parentPackage)
                    .ConfigureAwait(false);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            if (extension != null)
            {
                return extension.TrimStart('.');
            }
            return string.Empty;
        }


        async Task GetSnippets(IndexReader stringReader, string path, VersionRange parentVersion, string parentPackage)
        {
            var language = GetLanguageFromPath(path);
            var loopState = new LoopState();
            while (true)
            {
                var line = await stringReader.ReadLine()
                    .ConfigureAwait(false);
                if (line == null)
                {
                    if (loopState.IsInSnippet)
                    {
                        snippets.Add(new ReadSnippet(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: loopState.StartLine.Value + 1,
                            key: loopState.CurrentKey,
                            version: null,
                            package: null));
                    }
                    break;
                }

                var trimmedLine = line.Trim().Replace("  ", " ").ToLowerInvariant();
                if (loopState.IsInSnippet)
                {
                    if (!loopState.EndFunc(trimmedLine))
                    {
                        loopState.SnippetLines.Add(line);
                        continue;
                    }

                    var snippet = BuildSnippet(stringReader, path, loopState, language, parentVersion, parentPackage);

                    snippets.Add(snippet);
                    loopState.Reset();
                    continue;
                }
                StartEndTester.IsStart(stringReader, trimmedLine, loopState);
            }
        }


        ReadSnippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language, VersionRange parentVersion, string parentPackage)
        {
            VersionRange parsedVersion;
            var startRow = loopState.StartLine.Value + 1;

            string package;
            string error;
            if (!TryParseVersionAndPackage(path, loopState, parentVersion,parentPackage, out parsedVersion, out package, out error))
            {
                return new ReadSnippet(
                    error: error,
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey,
                    version: null,
                    package: package);
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $@"'{string.Join("', '", invalidCharacters)}'";
                return new ReadSnippet(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by you copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey,
                    version: parsedVersion,
                    package: package);
            }

            return new ReadSnippet(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.CurrentKey.ToLowerInvariant(),
                version: parsedVersion,
                value: value,
                path: path,
                package: package,
                language: language.ToLowerInvariant());
        }

        bool TryParseVersionAndPackage(string path, LoopState loopState, VersionRange parentVersion, string parentPackage, out VersionRange parsedVersion, out string package, out string error)
        {
            if (loopState.Suffix1 == null)
            {
                parsedVersion = versionExtractor(path, parentVersion);
                package = packageExtractor(path, parentPackage);
                error = null;
                return true;
            }

            if (loopState.Suffix2 == null)
            {
                // Suffix1 must be a version
                if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out parsedVersion))
                {
                    package = packageExtractor(path, parentPackage);
                    error = null;
                    return true;
                }

                // Suffix1 must be a package
                if (loopState.Suffix1.StartsWithLetter())
                {
                    parsedVersion = versionExtractor(path, parentVersion);
                    package = loopState.Suffix1;
                    error = null;
                    return true;
                }
                package = null;
                error = $"Expected '{loopState.Suffix2}' to be either parsable as a version or a package (starts with a letter).";
                return false;
            }

            if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out parsedVersion))
            {
                if (loopState.Suffix2.StartsWithLetter())
                {
                    package = loopState.Suffix2;
                    error = null;
                    return true;
                }
                error = $"Was able to parse '{loopState.Suffix1}' as a version. But a '{loopState.Suffix2}' is not a package, must starts with a letter.";
                package = null;
                return false;
            }

            if (VersionRangeParser.TryParseVersion(loopState.Suffix2, out parsedVersion))
            {
                if (loopState.Suffix1.StartsWithLetter())
                {
                    package = loopState.Suffix1;
                    error = null;
                    return true;
                }
                error = $"Was able to parse '{loopState.Suffix2}' as a version. But a '{loopState.Suffix1}' is not a package, must starts with a letter.";
                package = null;
                return false;
            }
            error = $"Was not able to parse either '{loopState.Suffix1}' or '{loopState.Suffix2}' as a version.";
            package = null;
            return false;
        }

        static string ConvertLinesToValue(List<string> snippetLines)
        {
            var snippetValue = snippetLines
                .ExcludeEmptyPaddingLines()
                .TrimIndentation();
            return string.Join(LineEnding, snippetValue);
        }


    }
}