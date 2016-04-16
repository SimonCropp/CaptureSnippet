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
        TranslatePackage translatePackage;
        ExtractMetaData extractMetaData;

        static char[] invalidCharacters = {'“', '”', '—', '`'};
        const string LineEnding = "\r\n";

        /// <summary>
        /// Initialise a new instance of <see cref="FileSnippetExtractor"/>.
        /// </summary>
        /// <param name="extractMetaData">How to extract a <see cref="SnippetMetaData"/> from a given path.</param>
        /// <param name="translatePackage">How to translate a package alias to the full package name.</param>
        public FileSnippetExtractor(ExtractMetaData extractMetaData, TranslatePackage translatePackage = null)
        {
            Guard.AgainstNull(extractMetaData, "extractMetaData");
            if (translatePackage == null)
            {
                this.translatePackage = alias => alias;
            }
            else
            {
                this.translatePackage = alias => InvokeTranslatePackage(translatePackage, alias);
            }
            this.extractMetaData = (path, parent) => InvokeExtractVersion(extractMetaData, path, parent);
        }


        static Result<SnippetMetaData> InvokeExtractVersion(ExtractMetaData extractMetaData, string path, SnippetMetaData parent)
        {
            path = path.Substring(0, path.LastIndexOf('.'));
            var result = extractMetaData(path, parent);
            if (result.Success)
            {
                if (result.Value == null)
                {
                    return Result<SnippetMetaData>.Failed($"ExtractMetaData supplied null for '{path}'.");
                }
                if (result.Value.Package.IsEmptyOrWhiteSpace())
                {
                    return Result<SnippetMetaData>.Failed($"ExtractMetaData returned empty string for '{path}'.");
                }
            }
            return result;
        }

        static Result<string> InvokeTranslatePackage(TranslatePackage translatePackage, string alias)
        {
            var result = translatePackage(alias);
            if (result.Success && string.IsNullOrWhiteSpace(result.Value))
            {
                return Result<string>.Failed($"TranslatePackage supplied an empty package for '{alias}'.");
            }
            return result;
        }


        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path so extract <see cref="ReadSnippet"/>s from.</param>
        /// <param name="parentMetaData">The inherited <see cref="SnippetMetaData"/>.</param>
        public async Task AppendFromReader(TextReader textReader, string path, SnippetMetaData parentMetaData, Action<ReadSnippet> callback)
        {
            Guard.AgainstNull(textReader, "textReader");
            using (var reader = new IndexReader(textReader))
            {
                await GetSnippets(reader, path, parentMetaData, callback)
                    .ConfigureAwait(false);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            return extension?.TrimStart('.') ?? string.Empty;
        }


        async Task GetSnippets(IndexReader stringReader, string path, SnippetMetaData parentMetaData, Action<ReadSnippet> callback)
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
                        callback(new ReadSnippet(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: loopState.StartLine.Value + 1,
                            key: loopState.CurrentKey,
                            version: null,
                            package: null));
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

                    var snippet = BuildSnippet(stringReader, path, loopState, language, parentMetaData);

                    callback(snippet);
                    loopState.Reset();
                    continue;
                }
                StartEndTester.IsStart(stringReader, trimmedLine, loopState);
            }
        }


        ReadSnippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language, SnippetMetaData parentMetaData)
        {
            var startRow = loopState.StartLine.Value + 1;

            string error;
            SnippetMetaData metaData;
            if (!TryParseVersionAndPackage(path, loopState, parentMetaData, out metaData, out error))
            {
                return new ReadSnippet(
                    error: error,
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey,
                    version: null,
                    package: metaData.Package);
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $@"'{string.Join("', '", invalidCharacters)}'";
                return new ReadSnippet(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey,
                    version: metaData.Version,
                    package: metaData.Package);
            }

            string translatedPackage = null;
            if (metaData.Package != null)
            {
                var translateResult = translatePackage(metaData.Package);
                if (!translateResult.Success)
                {
                    return new ReadSnippet(
                        error: $"Failed to translate package. Error: {translateResult.ErrorMessage}.",
                        path: path,
                        lineNumberInError: startRow,
                        key: loopState.CurrentKey,
                        version: metaData.Version,
                        package: metaData.Package);
                }
                translatedPackage = translateResult.Value;
            }
            return new ReadSnippet(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.CurrentKey,
                version: metaData.Version,
                value: value,
                path: path,
                package: translatedPackage,
                language: language.ToLowerInvariant());
        }

        bool TryParseVersionAndPackage(string path, LoopState loopState, SnippetMetaData parentMetaData, out SnippetMetaData parsedMetaData, out string error)
        {
            parsedMetaData = null;
            var metaDataForPath = extractMetaData(path, parentMetaData).Value;
            if (loopState.Suffix1 == null)
            {
                parsedMetaData = metaDataForPath;
                error = null;
                return true;
            }

            VersionRange parsedVersion;
            if (loopState.Suffix2 == null)
            {
                // Suffix1 must be a version
                if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out parsedVersion))
                {
                    parsedMetaData = new SnippetMetaData(parsedVersion, metaDataForPath.Package);
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

            if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out parsedVersion))
            {
                if (loopState.Suffix2.StartsWithLetter())
                {
                    parsedMetaData = new SnippetMetaData(parsedVersion, loopState.Suffix2);
                    error = null;
                    return true;
                }
                error = $"Was able to parse '{loopState.Suffix1}' as a version. But a '{loopState.Suffix2}' is not a package, must starts with a letter.";
                return false;
            }

            if (VersionRangeParser.TryParseVersion(loopState.Suffix2, out parsedVersion))
            {
                if (loopState.Suffix1.StartsWithLetter())
                {
                    parsedMetaData = new SnippetMetaData(parsedVersion, loopState.Suffix1);
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