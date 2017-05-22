using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CaptureSnippets.IncludeExtractors;

[DebuggerDisplay("Key={Key}, Version={Version}")]
class LoopState
{
    public string GetLines()
    {
        if (builder == null)
        {
            return string.Empty;
        }
        builder.TrimEnd();
        return builder.ToString();
    }

    public ISet<string> GetIncludes() => usings;

    public void AppendLine(string line)
    {
        AppendLine(line, NoOpUsingExtractor.Extract);
    }

    public void AppendLine(string line, Func<string, string> includeExtractor)
    {
        AppendContent(line);
        ExtractIncludes(includeExtractor, line);
    }

    private void ExtractIncludes(Func<string, string> includeExtractor, string line)
    {
        var include = includeExtractor(line);
        if (include != null)
        {
            usings.Add(include);
        }
    }

    private void AppendContent(string line)
    {
        if (builder == null)
        {
            builder = new StringBuilder();
        }
        if (builder.Length == 0)
        {
            if (line.IsWhiteSpace())
            {
                return;
            }
            CheckWhiteSpace(line, ' ');
            CheckWhiteSpace(line, '\t');
        }
        else
        {
            builder.AppendLine();
        }
        var paddingToRemove = line.LastIndexOfSequence(paddingChar, paddingLength);

        builder.Append(line, paddingToRemove, line.Length - paddingToRemove);
    }

    void CheckWhiteSpace(string line, char whiteSpace)
    {
        var c = line[0];
        if (c != whiteSpace)
        {
            return;
        }
        paddingChar = whiteSpace;
        for (var index = 1; index < line.Length; index++)
        {
            paddingLength++;
            var ch = line[index];
            if (ch != whiteSpace)
            {
                break;
            }
        }
    }

    StringBuilder builder;
    HashSet<string> usings = new HashSet<string>();
    public string Key;
    char paddingChar;
    int paddingLength;

    public string Version;
    public Func<string, bool> EndFunc;
    public int StartLine;
}