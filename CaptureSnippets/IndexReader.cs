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

        public static IndexReader FromFile(string path)
        {
            return new IndexReader(File.OpenText(path));
        }
        public static IndexReader FromString(string value)
        {
            return new IndexReader(new StringReader(value));
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
        public Task<string> ReadLineAsync()
        {
            Index++;
            return textReader.ReadLineAsync();
        }

    }
}