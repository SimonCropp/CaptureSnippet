using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MethodTimer;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="ReadSnippet"/>s from a given input.
    /// </summary>
    public class SnippetExtractor
    {
        Func<string, VersionRange> versionFromFilePathExtractor = s =>
        {
            throw new Exception("Failed to determine a version for a snippet. Please use the 'SnippetExtractor(Func<string, VersionRange> versionFromFilePathExtractor, Func<string, string> packageFromFilePathExtractor)' overload for to apply a fallback convention.");
        };
        Func<string, string> packageFromFilePathExtractor = s => null;

        static char[] invalidCharacters = {'“', '”', '—', '`'};
        const string LineEnding = "\r\n";

        /// <summary>
        /// Initialise a new insatnce of <see cref="SnippetExtractor"/>.
        /// </summary>
        /// <param name="versionFromFilePathExtractor">How to extract a <see cref="VersionRange"/> from a given file path.</param>
        /// <param name="packageFromFilePathExtractor">How to extract a package from a given file path. Return null for unknown.</param>
        public SnippetExtractor(Func<string, VersionRange> versionFromFilePathExtractor = null, Func<string, string> packageFromFilePathExtractor = null)
        {
            if (versionFromFilePathExtractor != null)
            {
                this.versionFromFilePathExtractor = versionFromFilePathExtractor;
            }
            if (packageFromFilePathExtractor != null)
            {
                this.packageFromFilePathExtractor = packageFromFilePathExtractor;
            }
        }

        [Time]
        public async Task<ReadSnippets> FromFiles(IEnumerable<string> files)
        {
            Guard.AgainstNull(files, "files");
            var tasks = files.Select(ProcessFile);
            var snippets = await Task.WhenAll(tasks).ConfigureAwait(false);
            return new ReadSnippets(
                snippets: snippets.SelectMany(x => x.Snippets),
                errors: snippets.SelectMany(x => x.Errors));
        }

        async Task<ReadSnippets> ProcessFile(string file)
        {
            using (var textReader = File.OpenText(file))
            using (var stringReader = new IndexReader(textReader))
            {
                return await GetSnippetsFromFile(stringReader, file).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Read <see cref="ReadSnippet"/> from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="source">Used to infer the version. Usually this will be the path to a file or a url.</param>
        public async Task<ReadSnippets> FromReader(TextReader textReader, string source = null)
        {
            Guard.AgainstNull(textReader, "textReader");
            using (var reader = new IndexReader(textReader))
            {
                return await GetSnippetsFromFile(reader, source).ConfigureAwait(false);
            }
        }

        static string GetLanguageFromFile(string file)
        {
            var extension = Path.GetExtension(file);
            if (extension != null)
            {
                return extension.TrimStart('.');
            }
            return string.Empty;
        }


        async Task<ReadSnippets> GetSnippetsFromFile(IndexReader stringReader, string file)
        {
            var errors = new List<ReadSnippetError>();
            var snippets = new List<ReadSnippet>();
            var language = GetLanguageFromFile(file);
            var loopState = new LoopState();
            while (true)
            {
                var line = await stringReader.ReadLine()
                    .ConfigureAwait(false);
                if (line == null)
                {
                    if (loopState.IsInSnippet)
                    {
                        errors.Add(new ReadSnippetError(
                            message: "Snippet was not closed",
                            file: file,
                            line: loopState.StartLine.Value + 1,
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

                    TryAddSnippet(stringReader, file, loopState, language, errors, snippets);
                    loopState.Reset();
                    continue;
                }
                IsStart(stringReader, trimmedLine, loopState);
            }
            return new ReadSnippets(snippets, errors);
        }

        static void IsStart(IndexReader stringReader, string trimmedLine, LoopState loopState)
        {
            string suffix1;
            string suffix2;
            string currentKey;
            if (IsStartCode(trimmedLine, out currentKey, out suffix1, out suffix2))
            {
                loopState.EndFunc = IsEndCode;
                loopState.CurrentKey = currentKey;
                loopState.IsInSnippet = true;
                loopState.Suffix1 = suffix1;
                loopState.Suffix2 = suffix2;
                loopState.StartLine = stringReader.Index;
                loopState.SnippetLines = new List<string>();
                return;
            }
            if (IsStartRegion(trimmedLine, out currentKey, out suffix1, out suffix2))
            {
                loopState.EndFunc = IsEndRegion;
                loopState.CurrentKey = currentKey;
                loopState.IsInSnippet = true;
                loopState.Suffix1 = suffix1;
                loopState.Suffix2 = suffix2;
                loopState.StartLine = stringReader.Index;
                loopState.SnippetLines = new List<string>();
            }
        }

        void TryAddSnippet(IndexReader stringReader, string file, LoopState loopState, string language, List<ReadSnippetError> errors, List<ReadSnippet> snippets)
        {
            VersionRange parsedVersion;
            var startRow = loopState.StartLine.Value + 1;

            string package;
            string error;
            if (!TryParseVersionAndPackage(file, loopState, out parsedVersion, out package, out error))
            {
                errors.Add(new ReadSnippetError(
                    message: error,
                    file: file,
                    line: startRow,
                    key: loopState.CurrentKey,
                    version: null,
                    package: package));
                return;
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $@"'{string.Join("', '", invalidCharacters)}'";
                errors.Add(new ReadSnippetError(
                    message: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by you copying code from MS Word or Outlook. Dont do that.",
                    file: file,
                    line: startRow,
                    key: loopState.CurrentKey,
                    version: parsedVersion,
                    package: package));
            }

            var snippet = new ReadSnippet(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.CurrentKey.ToLowerInvariant(),
                version: parsedVersion,
                value: value,
                file: file,
                package: package,
                language: language.ToLowerInvariant());
            snippets.Add(snippet);
        }

        bool TryParseVersionAndPackage(string file, LoopState loopState, out VersionRange parsedVersion, out string package, out string error)
        {
            if (loopState.Suffix1 == null)
            {
                if (GetVersionFromExtractor(file, out parsedVersion, out error))
                {
                    package = null;
                    error = null;
                    return true;
                }
                package = null;
                return false;
            }

            if (loopState.Suffix2 == null)
            {
                if (VersionRangeParser.TryParseVersion(loopState.Suffix1, out parsedVersion))
                {
                    package = null;
                    error = null;
                    return true;
                }
                if (loopState.Suffix1.StartsWithLetter())
                {
                    if (GetVersionFromExtractor(file, out parsedVersion, out error))
                    {
                        package = loopState.Suffix1;
                        return true;
                    }
                    package = null;
                    return false;
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

        bool GetVersionFromExtractor(string file, out VersionRange versionRange, out string error)
        {
            versionRange = versionFromFilePathExtractor(file);
            if (versionRange == null)
            {
                error ="Null version received from 'versionFromFilePathExtractor'.";
                return false;
            }
            error = null;
            return true;
        }

        static string ConvertLinesToValue(List<string> snippetLines)
        {
            var snippetValue = snippetLines
                .ExcludeEmptyPaddingLines()
                .TrimIndentation();
            return string.Join(LineEnding, snippetValue);
        }

        static bool IsEndRegion(string line)
        {
            return line.IndexOf("#endregion", StringComparison.Ordinal) >= 0;
        }

        static bool IsEndCode(string line)
        {
            return line.IndexOf("endcode", StringComparison.Ordinal) >= 0;
        }

        internal static bool IsStartRegion(string line, out string key, out string suffix1, out string suffix2)
        {
            if (!line.StartsWith("#region", StringComparison.Ordinal))
            {
                key = suffix2 = suffix1 = null;
                return false;
            }
            var substring = line.Substring(8);
            return TryExtractParts(out key, out suffix1, out suffix2, substring, line);
        }

        internal static bool IsStartCode(string line, out string key, out string suffix1, out string suffix2)
        {
            var startCodeIndex = line.IndexOf("startcode", StringComparison.Ordinal);
            if (startCodeIndex == -1)
            {
                key = suffix2 = suffix1 = null;
                return false;
            }
            var startIndex = startCodeIndex + 10;
            var substring = line.Substring(startIndex)
                .TrimBackCommentChars();
            return TryExtractParts(out key, out suffix1, out suffix2, substring, line);
        }

        static bool TryExtractParts(out string key, out string suffix1, out string suffix2, string substring, string line)
        {
            var split = substring.SplitBySpace();
            if (split.Length == 0)
            {
                throw new Exception($"No Key could be derived. Line: '{line}'.");
            }
            key = split[0];
            ValidateKeyDoesNotStartOrEndWithSymbol(key);
            if (split.Length == 1)
            {
                suffix2 = suffix1 = null;
                return true;
            }
            suffix1 = split[1];
            if (split.Length == 2)
            {
                suffix2 = null;
                return true;
            }
            if (split.Length == 3)
            {
                suffix2 = split[2];
                return true;
            }

            throw new Exception($"Too many parts. Line: '{line}'.");
        }

        static void ValidateKeyDoesNotStartOrEndWithSymbol(string key)
        {
            if (char.IsLetterOrDigit(key, 0) && char.IsLetterOrDigit(key, key.Length - 1))
            {
                return;
            }
            throw new Exception("Key should not start or end with symbols.");
        }

    }
}