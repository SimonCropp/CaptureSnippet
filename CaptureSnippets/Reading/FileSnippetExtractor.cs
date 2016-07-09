using System;
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
        ExtractFileNameData extractPathData;

        static char[] invalidCharacters = {'“', '”', '—'};

        /// <summary>
        /// Initialise a new instance of <see cref="FileSnippetExtractor"/>.
        /// </summary>
        /// <param name="extractPathData">How to extract a <see cref="PathData"/> from a given path.</param>
        public FileSnippetExtractor(ExtractFileNameData extractPathData)
        {
            Guard.AgainstNull(extractPathData, nameof(extractPathData));
            this.extractPathData = extractPathData;
        }

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path so extract <see cref="ReadSnippet"/>s from.</param>
        public async Task AppendFromReader(TextReader textReader, string path, VersionRange parentVersion, Package parentPackage, Component parentComponent, Action<ReadSnippet> callback)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(callback, nameof(callback));
            Guard.AgainstNull(parentVersion, nameof(parentVersion));
            Guard.AgainstNull(parentPackage, nameof(parentPackage));
            Guard.AgainstNull(parentComponent, nameof(parentComponent));
            using (var reader = new IndexReader(textReader))
            {
                await GetSnippets(reader, path, parentVersion, parentPackage, parentComponent, callback);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            return extension?.TrimStart('.') ?? string.Empty;
        }


        async Task GetSnippets(IndexReader stringReader, string path, VersionRange parentVersion, Package parentPackage, Component parentComponent, Action<ReadSnippet> callback)
        {
            VersionRange fileVersion;
            Package filePackage;
            Component fileComponent;
            PathDataExtractor.ExtractDataForFile(parentVersion, parentPackage, parentComponent, extractPathData, path, out fileVersion, out filePackage, out fileComponent);

            var language = GetLanguageFromPath(path);
            var loopState = new LoopState();
            while (true)
            {
                var line = await stringReader.ReadLine();
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
                        loopState.AppendLine(line);
                        continue;
                    }

                    var snippet = BuildSnippet(stringReader, path, loopState, language, fileVersion, filePackage, fileComponent);

                    callback(snippet);
                    loopState.Reset();
                    continue;
                }
                string suffix1;
                string suffix2;
                Func<string, bool> endFunc;
                string key;
                if (StartEndTester.IsStart(trimmedLine, out suffix1, out suffix2, out key, out endFunc))
                {
                    loopState.EndFunc = endFunc;
                    loopState.CurrentKey = key;
                    loopState.IsInSnippet = true;
                    loopState.Suffix1 = suffix1;
                    loopState.Suffix2 = suffix2;
                    loopState.StartLine = stringReader.Index;
                }
            }
        }


        ReadSnippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language, VersionRange fileVersion, Package filePackage, Component fileComponent)
        {
            var startRow = loopState.StartLine.Value + 1;

            string error;
            Package snippetPackage;
            VersionRange snippetVersion;
            if (!TryParseVersionAndPackage(loopState, fileVersion, filePackage, out snippetVersion, out snippetPackage, out error))
            {
                return new ReadSnippet(
                    error: error,
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey);
            }
            var value = loopState.GetLines();
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
                component: fileComponent,
                language: language.ToLowerInvariant());
        }

        bool TryParseVersionAndPackage(LoopState loopState, VersionRange fileVersion, Package filePackage, out VersionRange snippetVersion, out Package snippetPackage, out string error)
        {
            snippetVersion = null;
            snippetPackage = Package.Undefined;
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
                error = $"Expected '{loopState.Suffix1}' to be either parsable as a version or a package (starts with a letter).";
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

    }
}
