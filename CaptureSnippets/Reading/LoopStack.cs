using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("Depth={stack.Count}, IsInSnippet={IsInSnippet}")]
class LoopStack
{
    Stack<LoopState> stack = new Stack<LoopState>();
    Func<string, string> includeExtractor;

    public LoopStack(Func<string, string> includeExtractor)
    {
        this.includeExtractor = includeExtractor;
    }

    public bool IsInSnippet => stack.Count > 0;

    public LoopState Current => stack.Peek();

    public void AppendLine(string line)
    {
        foreach (var state in stack)
        {
            state.AppendLine(line, includeExtractor);
        }
    }

    public void Pop()
    {
        stack.Pop();
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
        stack.Push(state);
    }
}