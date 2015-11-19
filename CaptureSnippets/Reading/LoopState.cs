using System;
using System.Collections.Generic;

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
    public string CurrentKey;

    public string Version;
    public Func<string, bool> EndFunc;
    public int? StartLine;
    public bool IsInSnippet;
}