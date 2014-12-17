using System;
using System.IO;

namespace CaptureSnippets
{
    public class IndexReader:IDisposable
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
            textReader.Dispose();
        }

        public string ReadLine()
        {
            Index++;

            //if (peeked != null)
            //{
            //    return peeked;
            //}
            return textReader.ReadLine();
        }

        //string peeked;
        //public string PeekLine()
        //{
        //    if (peeked == null)
        //    {
        //        peeked = textReader.ReadLine();
        //    }
        //    return peeked;
        //}
    }
}