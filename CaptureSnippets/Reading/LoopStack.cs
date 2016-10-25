using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("Depth={Stack.Count}, IsInSnippet={IsInSnippet}")]
class LoopStack
{
    public Stack<LoopState> Stack = new Stack<LoopState>();

    public bool IsInSnippet => Stack.Count > 0;

    public LoopState Current => Stack.Peek();

    public void AppendLine(string line)
    {
        foreach (var state in Stack)
        {
            state.AppendLine(line);
        }
    }

    public void Pop()
    {
        Stack.Pop();
    }

    public void Push(Func<string, bool> endFunc, string key, string version, int startLine)
    {
        var state = new LoopState
        {
            Version = version,
            Key = key,
            EndFunc = endFunc,
            StartLine = startLine,
        };
        Stack.Push(state);
    }
}