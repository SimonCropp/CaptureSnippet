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
            throw new Exception("Failed to determin a version for a snippet. Please use the 'SnippetExtractor(Func<string, VersionRange> versionFromFilePathExtractor)' overload for to apply a fallback convention.");
        };

        static char[] invalidCharacters = {'“', '”', '—','`' };
        const string LineEnding = "\r\n";

        /// <summary>
        /// Initialise a new insatnce of <see cref="SnippetExtractor"/>.
        /// </summary>
        public SnippetExtractor()
        {
            
        }

        /// <summary>
        /// Initialise a new insatnce of <see cref="SnippetExtractor"/>.
        /// </summary>
        /// <param name="versionFromFilePathExtractor">How to extract a <see cref="VersionRange"/> from a given file path. Return null for unknown version.</param>
        public SnippetExtractor(Func<string, VersionRange> versionFromFilePathExtractor)
        {
            Guard.AgainstNull(versionFromFilePathExtractor, "versionFromFilePathExtractor");
            this.versionFromFilePathExtractor = versionFromFilePathExtractor;
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
                var line = await stringReader.ReadLine().ConfigureAwait(false);
                if (line == null)
                {
                    if (loopState.IsInSnippet)
                    {
                        errors.Add(new ReadSnippetError(
                            message: "Snippet was not closed",
                            file: file,
                            line: loopState.StartLine.Value + 1,
                            key: loopState.CurrentKey, 
                            version: null));
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
            string version;
            string currentKey;
            if (IsStartCode(trimmedLine, out currentKey, out version))
            {
                loopState.EndFunc = IsEndCode;
                loopState.CurrentKey = currentKey;
                loopState.IsInSnippet = true;
                loopState.Version = version;
                loopState.StartLine = stringReader.Index;
                loopState.SnippetLines = new List<string>();
                return;
            }
            if (IsStartRegion(trimmedLine, out currentKey, out version))
            {
                loopState.EndFunc = IsEndRegion;
                loopState.CurrentKey = currentKey;
                loopState.IsInSnippet = true;
                loopState.Version = version;
                loopState.StartLine = stringReader.Index;
                loopState.SnippetLines = new List<string>();
            }
        }

        void TryAddSnippet(IndexReader stringReader, string file, LoopState loopState, string language, List<ReadSnippetError> errors, List<ReadSnippet> snippets)
        {
            VersionRange parsedVersion;
            var startRow = loopState.StartLine.Value + 1;
            
            if (!TryParseVersion(file, loopState, out parsedVersion))
            {
                errors.Add(new ReadSnippetError(
                                            message : "Could not extract version",
                                            file : file,
                                            line : startRow,
                                            key : loopState.CurrentKey,
                                            version:null));
                return;
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = "'" + string.Join("', '", invalidCharacters) + "'";
                errors.Add(new ReadSnippetError(
                    message: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by you copying code from MS Word or Outlook. Dont do that.",
                    file: file,
                    line: startRow,
                    key: loopState.CurrentKey,
                    version: parsedVersion));
            }
            
            var snippet = new ReadSnippet(
                              startLine : startRow,
                              endLine : stringReader.Index,
                              key : loopState.CurrentKey.ToLowerInvariant(),
                              version : parsedVersion,
                              value : value,
                              file : file,
                              language: language.ToLowerInvariant());
            snippets.Add(snippet);
        }

        bool TryParseVersion(string file, LoopState loopState, out VersionRange parsedVersion)
        {
            var stringVersion = loopState.Version;
            if (stringVersion == null)
            {
                parsedVersion = versionFromFilePathExtractor(file);
                if (parsedVersion == null)
                {
                    throw new Exception("Null version received from 'versionFromFilePathExtractor'. Did you mean to use 'CaptureSnippets.Version.ExplicitEmpty'.");
                }
                return true;
            }
            return VersionRangeParser.TryParseVersion(stringVersion, out parsedVersion);
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

        internal static bool IsStartRegion(string line, out string key, out string version)
        {
            return Extract(line, out key, out version, "#region");
        }

        internal static bool IsStartCode(string line, out string key, out string version)
        {
            return Extract(line.Replace("-->",""), out key, out version, "startcode");
        }

        static bool Extract(string line, out string key, out string version, string prefix)
        {
            var startCodeIndex = line.IndexOf(prefix + " ", StringComparison.Ordinal);
            if (startCodeIndex != -1)
            {
                var startIndex = startCodeIndex + prefix.Length +1;

                var substring = line.Substring(startIndex);
                var splitBySpace = substring
                    .Split(new[]
                           {
                               ' '
                           }, StringSplitOptions.RemoveEmptyEntries);
                if (splitBySpace.Any())
                {
                    key = splitBySpace[0]
                        .TrimNonCharacters();
                    if (splitBySpace.Length > 1)
                    {
                        version = splitBySpace[1];
                    }
                    else
                    {
                        version = null;
                    }
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        return true;
                    }
                }
                throw new Exception("No Key could be derived.");
            }
            version = null;
            key = null;
            return false;
        }
    }
}
