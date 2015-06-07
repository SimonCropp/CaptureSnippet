using System;
using System.IO;

namespace CaptureSnippets
{
    class IndexReader:IDisposable
    {
        TextReader textReader;
        public int Index { get; private set; }
        public IndexReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public void Dispose()
        {
            if (textReader != null)
            {
                textReader.Dispose();
            }
        }

        public string ReadLine()
        {
            Index++;
            return textReader.ReadLine();
        }

    }
}