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
            this.extractMetaDataFromPath = (rootPath, path, parent) => InvokeExtractVersion(extractMetaDataFromPath, rootPath, path, parent);
        }


        static Result<SnippetMetaData> InvokeExtractVersion(ExtractMetaDataFromPath extractMetaDataFromPath, string rootPath, string path, SnippetMetaData parent)
        {
            path = path.Substring(0, path.LastIndexOf('.'));
            var result = extractMetaDataFromPath(rootPath, path, parent);
            if (result.Success)
            {
                if (result.Value == null)
                {
                    return Result<SnippetMetaData>.Failed($"ExtractMetaData supplied null for '{path}'.");
                }
            }
            return result;
        }


        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path so extract <see cref="ReadSnippet"/>s from.</param>
        /// <param name="parentMetaData">The inherited <see cref="SnippetMetaData"/>.</param>
        public async Task AppendFromReader(TextReader textReader, string rootPath, string path, SnippetMetaData parentMetaData, Action<ReadSnippet> callback)
        {
            Guard.AgainstNull(textReader, "textReader");
            using (var reader = new IndexReader(textReader))
            {
                await GetSnippets(reader, rootPath, path, parentMetaData, callback)
                    .ConfigureAwait(false);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            return extension?.TrimStart('.') ?? string.Empty;
        }


        async Task GetSnippets(IndexReader stringReader, string rootPath, string path, SnippetMetaData parentMetaData, Action<ReadSnippet> callback)
        {
            var metaDataForPath =  new Lazy<SnippetMetaData>(() => extractMetaDataFromPath(rootPath, path, parentMetaData).Value);
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

                    var snippet = BuildSnippet(stringReader, path, loopState, language, metaDataForPath);

                    callback(snippet);
                    loopState.Reset();
                    continue;
                }
                StartEndTester.IsStart(stringReader, trimmedLine, loopState);
            }
        }


        ReadSnippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language, Lazy<SnippetMetaData> metaDataForPath)
        {
            var startRow = loopState.StartLine.Value + 1;

            string error;
            SnippetMetaData metaData;
            if (!TryParseVersionAndPackage(loopState, metaDataForPath, out metaData, out error))
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
                version: metaData.Version,
                value: value,
                path: path,
                package: metaData.Package,
                language: language.ToLowerInvariant());
        }

        bool TryParseVersionAndPackage(LoopState loopState, Lazy<SnippetMetaData> lazyMetaDataForPath, out SnippetMetaData parsedMetaData, out string error)
        {
            var metaDataForPath = lazyMetaDataForPath.Value;
            parsedMetaData = null;
            if (loopState.Suffix1 == null)
            {
                parsedMetaData = metaDataForPath;
                error = null;
                return true;
            }

                VersionRange version;
            if (loopState.Suffix2 == null)
            {
                // Suffix1 must be a version
                if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out version))
                {
                    parsedMetaData = new SnippetMetaData(version, metaDataForPath.Package);
                    error = null;
                    return true;
                }

                // Suffix1 must be a package
                if (loopState.Suffix1.StartsWithLetter())
                {
                    parsedMetaData = new SnippetMetaData(metaDataForPath.Version, loopState.Suffix1);
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
                    parsedMetaData = new SnippetMetaData(version, loopState.Suffix2);
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
                    parsedMetaData = new SnippetMetaData(version, loopState.Suffix1);
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