using System;
using System.Collections.Generic;
using System.IO;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="Snippet"/>s from a given input.
    /// </summary>
    public class FileSnippetExtractor
    {
        VersionRange fileVersion;
        string package;

        public static FileSnippetExtractor Build(VersionRange fileVersion, string package, bool isCurrent)
        {
            Guard.AgainstNull(fileVersion, nameof(fileVersion));
            Guard.AgainstNullAndEmpty(package, nameof(package));
            return new FileSnippetExtractor
            {
                fileVersion = fileVersion,
                package = package,
                isCurrent = isCurrent
            };
        }

        public static FileSnippetExtractor BuildShared()
        {
            return new FileSnippetExtractor
            {
                isShared = true
            };
        }

        static char[] invalidCharacters = { '“', '”', '—' };
        bool isShared;
        bool isCurrent;

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path so extract <see cref="Snippet"/>s from.</param>
        public IEnumerable<Snippet> AppendFromReader(TextReader textReader, string path)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNullAndEmpty(path, nameof(path));
            try
            {
                var reader = new IndexReader(textReader);
                return GetSnippets(reader, path);
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not extract snippets from '{path}';", exception);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            return extension?.TrimStart('.') ?? string.Empty;
        }


        IEnumerable<Snippet> GetSnippets(IndexReader stringReader, string path)
        {
            var language = GetLanguageFromPath(path);
            var loopState = new LoopState();
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (loopState.IsInSnippet)
                    {
                        yield return Snippet.BuildError(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: loopState.StartLine.Value + 1,
                            key: loopState.CurrentKey);
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

                    yield return BuildSnippet(stringReader, path, loopState, language);
                    loopState.Reset();
                    continue;
                }
                string version;
                Func<string, bool> endFunc;
                string key;
                if (StartEndTester.IsStart(trimmedLine, out version, out key, out endFunc))
                {
                    loopState.EndFunc = endFunc;
                    loopState.CurrentKey = key;
                    loopState.IsInSnippet = true;
                    loopState.Version = version;
                    loopState.StartLine = stringReader.Index;
                }
            }
        }


        Snippet BuildSnippet(IndexReader stringReader, string path, LoopState loopState, string language)
        {
            var startRow = loopState.StartLine.Value + 1;

            string error;
            VersionRange snippetVersion;
            if (!TryParseVersionAndPackage(loopState, out snippetVersion, out error))
            {
                return Snippet.BuildError(
                    error: error,
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey);
            }
            var value = loopState.GetLines();
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $@"'{string.Join("', '", invalidCharacters)}'";
                return Snippet.BuildError(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.CurrentKey);
            }
            if (isShared)
            {
                return Snippet.BuildShared(
                    startLine: startRow,
                    endLine: stringReader.Index,
                    key: loopState.CurrentKey,
                    value: value,
                    path: path,
                    language: language.ToLowerInvariant());
            }
            return Snippet.Build(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.CurrentKey,
                version: snippetVersion,
                value: value,
                path: path,
                language: language.ToLowerInvariant(),
                package: package,
                isCurrent: isCurrent);
        }

        bool TryParseVersionAndPackage(LoopState loopState, out VersionRange snippetVersion, out string error)
        {
            snippetVersion = null;
            if (loopState.Version == null)
            {
                snippetVersion = fileVersion;
                error = null;
                return true;
            }

            VersionRange version;
            if (VersionRangeParser.TryParseVersion(loopState.Version, out version))
            {
                if (fileVersion.IsPreRelease())
                {
                    error = $"Could not use '{loopState.Version}' since directory is flagged as Prerelease. FileVersion: {fileVersion.ToFriendlyString()}.";
                    return false;
                }
                snippetVersion = version;

                error = null;
                return true;
            }

            error = $"Expected '{loopState.Version}' to be either parsable as a version.";
            return false;
        }

    }
}