using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MethodTimer;

namespace CaptureSnippets
{
    public class SnippetExtractor
    {
        Func<string, Version> versionFromFilePathExtractor;
        const string LineEnding = "\r\n";

        public SnippetExtractor():this(s => null)
        {
            
        }
        public SnippetExtractor(Func<string, Version> versionFromFilePathExtractor)
        {
            this.versionFromFilePathExtractor = versionFromFilePathExtractor;
        }

        [Time]
        public ReadSnippets FromFiles(IEnumerable<string> files)
        {
            var readSnippets = new ReadSnippets();
            foreach (var file in files)
            {
                using (var stringReader = IndexReader.FromFile(file))
                {
                    GetSnippetsFromFile(readSnippets, stringReader, file);
                }
            }
            return readSnippets;
        }

        public ReadSnippets FromText(string contents, string file = null)
        {
            var readSnippets = new ReadSnippets();
            using (var reader = IndexReader.FromString(contents))
            {
                GetSnippetsFromFile(readSnippets, reader, file);
            }
            return readSnippets;
        }

        static string GetLanguageFromFile(string file)
        {
            var extension = Path.GetExtension(file);
            if (extension != null)
            {
                return extension.TrimStart('.');
            }
            return String.Empty;
        }

        class LoopState
        {
            public void Reset()
            {

                SnippetLines = null;
                CurrentKey = null;
                Version = null;
                EndFunc = null;
                StartLine = null;
                IsInSnippet = false;
            }

            public List<string> SnippetLines;
            public string CurrentKey { get; set; }

            public string Version { get; set; }
            public Func<string, bool> EndFunc;
            public int? StartLine;
            public bool IsInSnippet;
        }

        void GetSnippetsFromFile(ReadSnippets readSnippets,IndexReader stringReader, string file)
        {
            var language = GetLanguageFromFile(file);
            var loopState = new LoopState();
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (loopState.IsInSnippet)
                    {
                        var error = string.Format("Snippet was not closed. File:`{0}`. Line:{1}. Key:`{2}`", file ?? "unknown", loopState.StartLine.Value+1, loopState.CurrentKey);
                        readSnippets.Errors.Add(error);
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
                    
                    TryAddSnippet(readSnippets, stringReader, file, loopState, language);
                    loopState.Reset();
                    continue;
                }
                IsStart(stringReader, trimmedLine, loopState);
            }
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
            }
            else if (IsStartRegion(trimmedLine, out currentKey, out version))
            {
                loopState.EndFunc = IsEndRegion;
                loopState.CurrentKey = currentKey;
                loopState.IsInSnippet = true;
                loopState.Version = version;
                loopState.StartLine = stringReader.Index;
                loopState.SnippetLines = new List<string>();
            }
        }

        void TryAddSnippet(ReadSnippets readSnippets, IndexReader stringReader, string file, LoopState loopState, string language)
        {
            Version parsedVersion;
            var startRow = loopState.StartLine.Value + 1;
            if (!TryParseVersion(file, loopState, out parsedVersion))
            {
                var error = string.Format("Could not extract version from {0}. File:`{1}`. Line:{2}. Key:`{3}`", loopState.Version, file ?? "unknown", startRow, loopState.CurrentKey);
                readSnippets.Errors.Add(error);
                return;
            }
            if (readSnippets.Snippets.Any(x => x.Key == loopState.CurrentKey && x.Version == parsedVersion && x.Language == language))
            {
                var error = string.Format("Duplicate key detected. File:`{0}`. Line:{1}. Key:`{2}`. Version:`{3}`", file ?? "unknown", startRow, loopState.CurrentKey,parsedVersion);
                readSnippets.Errors.Add(error);
                return;
            }
            var value = ConvertLinesToValue(loopState.SnippetLines);
            if (value.Contains('`'))
            {
                var error = string.Format("Sippet contains a code quote character. File:`{0}`. Line:{1}. Key:`{2}`. Version:`{3}`", file ?? "unknown", startRow, loopState.CurrentKey, parsedVersion);
                readSnippets.Errors.Add(error);
                return;
            }
            var snippet = new ReadSnippet
                          {
                              StartRow = startRow,
                              EndRow = stringReader.Index,
                              Key = loopState.CurrentKey,
                              Version = parsedVersion,
                              Value = value,
                              File = file,
                              Language = language,
                          };
            readSnippets.Snippets.Add(snippet);
        }

        bool TryParseVersion(string file, LoopState loopState, out Version parsedVersion)
        {
            var stringVersion = loopState.Version;
            if (stringVersion == null)
            {
                parsedVersion = versionFromFilePathExtractor(file);
                return true;
            }
            return VersionParser.TryParseVersion(stringVersion, out parsedVersion);
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

        public static bool IsStartRegion(string line, out string key, out string version)
        {
            return Extract(line, out key, out version, "#region");
        }

        public static bool IsStartCode(string line, out string key, out string version)
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
