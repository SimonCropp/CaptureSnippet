using System;
using System.IO;
using System.Threading.Tasks;

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

        public Task<string> ReadLineAsync()
        {
            Index++;
            return textReader.ReadLineAsync();
        }

    }
}