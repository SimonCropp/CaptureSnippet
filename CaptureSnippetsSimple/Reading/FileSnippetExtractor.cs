﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    /// <summary>
    /// Extracts <see cref="Snippet"/>s from a given input.
    /// </summary>
    public class FileSnippetExtractor
    {
        /// <summary>
        /// Read from a path.
        /// </summary>
        /// <param name="path">The current path to extract <see cref="Snippet"/>s from.</param>
        public IEnumerable<Snippet> Read(string path)
        {
            Guard.AgainstNullAndEmpty(path, nameof(path));
            using (var reader = File.OpenText(path))
            {
                return Read(reader, path);
            }
        }

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path being used to extract <see cref="Snippet"/>s from. Only used for logging purposes in this overload.</param>
        public IEnumerable<Snippet> Read(TextReader textReader, string path)
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
            var loopStack = new LoopStack();

            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (loopStack.IsInSnippet)
                    {
                        var current = loopStack.Current;
                        yield return Snippet.BuildError(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: current.StartLine + 1,
                            key: current.Key);
                    }
                    break;
                }

                var trimmedLine = line.Trim()
                    .Replace("  ", " ")
                    .ToLowerInvariant();

                if (StartEndTester.IsStart(trimmedLine, out var key, out var endFunc))
                {
                    loopStack.Push(endFunc, key, stringReader.Index);
                    continue;
                }
                if (loopStack.IsInSnippet)
                {
                    if (!loopStack.Current.EndFunc(trimmedLine))
                    {
                        loopStack.AppendLine(line);
                        continue;
                    }

                    yield return BuildSnippet(stringReader, path, loopStack, language);
                    loopStack.Pop();
                }
            }
        }

        Snippet BuildSnippet(IndexReader stringReader, string path, LoopStack loopStack, string language)
        {
            var loopState = loopStack.Current;
            var startRow = loopState.StartLine + 1;

            var value = loopState.GetLines();
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $"'{string.Join("', '", invalidCharacters)}'";
                return Snippet.BuildError(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.Key);
            }

            return Snippet.Build(
                startLine: startRow,
                endLine: stringReader.Index,
                key: loopState.Key,
                value: value,
                path: path,
                language: language.ToLowerInvariant()
            );
        }

        static char[] invalidCharacters = { '“', '”', '—' };
    }
}