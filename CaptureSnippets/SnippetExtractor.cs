using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MethodTimer;


namespace CaptureSnippets
{
    public class SnippetExtractor
    {
        Func<string, string> versionFromFilePathExtractor;
        const string LineEnding = "\r\n";

        public SnippetExtractor():this(s => null)
        {
            
        }
        public SnippetExtractor(Func<string,string> versionFromFilePathExtractor)
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

        void GetSnippetsFromFile(ReadSnippets readSnippets,IndexReader stringReader, string file)
        {
            var language = GetLanguageFromFile(file);
            string currentKey = null;
            var startLine = 0;
            var isInSnippet = false;
            string version = null;
            List<string> snippetLines = null;
            Func<string, bool> endFunc = null;
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (isInSnippet)
                    {
                        var error = string.Format("Snippet was not closed. File:`{0}`. Line:{1}. Key:`{2}`", file ?? "unknown", startLine + 1, currentKey);
                        readSnippets.Errors.Add(error);
                    }
                    break;
                }

                var trimmedLine = line.Trim().Replace("  ", " ").ToLowerInvariant();
                if (isInSnippet)
                {
                    if (!endFunc(trimmedLine))
                    {
                        snippetLines.Add(line);
                        continue;
                    }
                    isInSnippet = false;

                    if (readSnippets.Snippets.Any(x => x.Key == currentKey && x.Version == version && x.Language == language))
                    {
                        var error = string.Format("Duplicate key detected. File:`{0}`. Line:{1}. Key:`{2}`", file ?? "unknown", startLine + 1, currentKey);
                        readSnippets.Errors.Add(error);
                    }
                    else
                    {
                        var snippet = new ReadSnippet
                                      {
                                          StartRow = startLine + 1,
                                          EndRow = stringReader.Index,
                                          Key = currentKey,
                                          Version = CheckVersionConvention(file,version),
                                          Value = ConvertLinesToValue(snippetLines),
                                          File = file,
                                          Language = language,
                                      };
                        readSnippets.Snippets.Add(snippet);
                    }
                    snippetLines = null;
                    currentKey = null;
                    version = null;
                    continue;
                }
                if (IsStartCode(trimmedLine, out currentKey, out version))
                {
                    endFunc = IsEndCode;
                    isInSnippet = true;
                    startLine = stringReader.Index;
                    snippetLines = new List<string>();
                   continue;
                }
                if (IsStartRegion(trimmedLine, out currentKey, out version))
                {
                    endFunc = IsEndRegion;
                    isInSnippet = true;
                    startLine = stringReader.Index;
                    snippetLines = new List<string>();
                }
            }
        }

        string CheckVersionConvention(string filePath, string version)
        {
            if (version == null)
            {
                return versionFromFilePathExtractor(filePath);
            }

                return version;
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
