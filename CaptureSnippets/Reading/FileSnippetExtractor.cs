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
        ExtractMetaDataFromPath extractMetaDataFromPath;

        static char[] invalidCharacters = {'“', '”', '—', '`'};
        const string LineEnding = "\r\n";

        /// <summary>
        /// Initialise a new instance of <see cref="FileSnippetExtractor"/>.
        /// </summary>
        /// <param name="extractMetaDataFromPath">How to extract a <see cref="SnippetMetaData"/> from a given path.</param>
        public FileSnippetExtractor(ExtractMetaDataFromPath extractMetaDataFromPath)
        {
            Guard.AgainstNull(extractMetaDataFromPath, "extractMetaData");
            this.extractMetaDataFromPath = extractMetaDataFromPath;
        }

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path so extract <see cref="ReadSnippet"/>s from.</param>
        public async Task AppendFromReader(TextReader textReader, string rootPath, string path, VersionRange parentVersion, Package parentPackage, Action<ReadSnippet> callback)
        {
            Guard.AgainstNull(textReader, "textReader");
            using (var reader = new IndexReader(textReader))
            {
                await GetSnippets(reader, rootPath, path, parentVersion, parentPackage, callback)
                    .ConfigureAwait(false);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            return extension?.TrimStart('.') ?? string.Empty;
        }


        async Task GetSnippets(IndexReader stringReader, string rootPath, string path, VersionRange parentVersion, Package parentPackage, Action<ReadSnippet> callback)
        {
            VersionRange fileVersion;
            Package filePackage;
            var pathWithoutExtension = path.Substring(0, path.LastIndexOf('.'));
            var metaDataFromPath = extractMetaDataFromPath;
            MetadataExtractor.ExtractVersionAndPackage(rootPath, parentVersion, parentPackage, metaDataFromPath, pathWithoutExtension, out fileVersion, out filePackage);

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
                        callback(new ReadSnippet(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: loopState.StartLine.Value + 1,
                            key: loopState.CurrentKey));
                    }
                    break;
                }

                var trimmedLine = line.Trim()
                    .Replace("  ", " ")
                    .ToLowerInvariant();
                if (loopState.IsInSnippet)
                {
                    if (!loopState.EndFunc(trimmedLine))
                    {
                        loopState.SnippetLines.Add(line);
                        continue;
                    }

                    var snippet = BuildSnippet(stringReader, path, loopState, language, fileVersion, filePackage);

                    callback(snippet);
                    loopState.Reset();
                    continue;
                }
                StartEndTester.IsStart(stringReader, trimmedLine, loopState);
            }
        }


        ReadSnippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language, VersionRange fileVersion, Package filePackage)
        {
            var startRow = loopState.StartLine.Value + 1;

            string error;
            Package snippetPackage;
            VersionRange snippetVersion;
            if (!TryParseVersionAndPackage(loopState, fileVersion,filePackage, out snippetVersion, out snippetPackage, out error))
            {
                return new ReadSnippet(
                    error: error,
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey);
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $@"'{string.Join("', '", invalidCharacters)}'";
                return new ReadSnippet(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey);
            }

            return new ReadSnippet(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.CurrentKey,
                version: snippetVersion,
                value: value,
                path: path,
                package: snippetPackage,
                language: language.ToLowerInvariant());
        }

        bool TryParseVersionAndPackage(LoopState loopState, VersionRange fileVersion, Package filePackage, out VersionRange snippetVersion, out Package snippetPackage, out string error)
        {
            snippetVersion = null;
            snippetPackage = null;
            if (loopState.Suffix1 == null)
            {
                snippetVersion = fileVersion;
                snippetPackage = filePackage;
                error = null;
                return true;
            }

            VersionRange version;
            if (loopState.Suffix2 == null)
            {
                // Suffix1 could be a version
                if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out version))
                {
                    snippetVersion = version;
                    snippetPackage = filePackage;
                    error = null;
                    return true;
                }

                // Suffix1 must be a package
                if (loopState.Suffix1.StartsWithLetter())
                {
                    snippetVersion = fileVersion;
                    snippetPackage = loopState.Suffix1;
                    error = null;
                    return true;
                }
                error = $"Expected '{loopState.Suffix2}' to be either parsable as a version or a package (starts with a letter).";
                return false;
            }

            if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out version))
            {
                if (loopState.Suffix2.StartsWithLetter())
                {
                    snippetVersion = version;
                    snippetPackage = loopState.Suffix2;
                    error = null;
                    return true;
                }
                error = $"Was able to parse '{loopState.Suffix1}' as a version. But a '{loopState.Suffix2}' is not a package, must starts with a letter.";
                return false;
            }

            if (VersionRangeParser.TryParseVersion(loopState.Suffix2, out version))
            {
                if (loopState.Suffix1.StartsWithLetter())
                {
                    snippetVersion = version;
                    snippetPackage = loopState.Suffix1;
                    error = null;
                    return true;
                }
                error = $"Was able to parse '{loopState.Suffix2}' as a version. But a '{loopState.Suffix1}' is not a package, must starts with a letter.";
                return false;
            }
            error = $"Was not able to parse either '{loopState.Suffix1}' or '{loopState.Suffix2}' as a version.";
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
