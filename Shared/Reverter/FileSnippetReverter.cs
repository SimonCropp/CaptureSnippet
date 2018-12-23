using System;
using System.IO;
using System.Text;

namespace CaptureSnippets
{
    /// <summary>
    /// Revers <see cref="Snippet"/>s from a given input.
    /// </summary>
    public static class FileSnippetReverter
    {
        public static string Revert(string input)
        {
            Guard.AgainstNull(input, nameof(input));
            var builder = new StringBuilder();
            using (var reader = new StringReader(input))
            using (var writer = new StringWriter(builder))
            {
                Revert(reader, writer);
            }

            return builder.ToString();
        }

        public static void Revert(TextReader reader, TextWriter writer)
        {
            Guard.AgainstNull(reader, nameof(reader));
            Guard.AgainstNull(writer, nameof(writer));
            var indexReader = new IndexReader(reader);
            var isInSnippet = false;
            string key = null;
            while (true)
            {
                var line = indexReader.ReadLine();

                if (isInSnippet)
                {
                    if (line == null)
                    {
                        throw new Exception($"Snippet was not closed. Key: {key}. Line: {indexReader.Index}");
                    }

                    if (StartEndCommentTester.IsEndCode(line))
                    {
                        isInSnippet = false;
                    }

                    continue;
                }

                if (line == null)
                {
                    break;
                }

                if (StartEndCommentTester.IsStart(line, out key))
                {
                    isInSnippet = true;
                    writer.WriteLine("snippet: " + key);
                    continue;
                }

                if (indexReader.IsEnd())
                {
                    writer.Write(line);
                    continue;
                }

                writer.WriteLine(line);
            }
        }
    }
}