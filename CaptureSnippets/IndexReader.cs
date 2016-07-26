using System;
using System.IO;

class IndexReader : IDisposable
{
    TextReader textReader;
    public int Index { get; private set; }

    public IndexReader(TextReader textReader)
    {
        this.textReader = textReader;
    }

    public void Dispose()
    {
        textReader?.Dispose();
    }

    public string ReadLine()
    {
        Index++;
        return textReader.ReadLine();
    }

}