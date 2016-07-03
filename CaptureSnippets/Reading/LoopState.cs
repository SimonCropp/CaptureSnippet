using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("CurrentKey={CurrentKey}, Suffix1={Suffix1}, Suffix2={Suffix2}, IsInSnippet={IsInSnippet}")]
struct LoopState
{
    public void Reset()
    {
        SnippetLines = null;
        CurrentKey = null;
        EndFunc = null;
        StartLine = null;
        IsInSnippet = false;
    }

    public List<string> SnippetLines;
    public string CurrentKey;

    public string Suffix1;
    public string Suffix2;
    public Func<string, bool> EndFunc;
    public int? StartLine;
    public bool IsInSnippet;
}