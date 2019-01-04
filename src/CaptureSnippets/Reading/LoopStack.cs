using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("Depth={stack.Count}, IsInSnippet={IsInSnippet}")]
class LoopStack
{
    public bool IsInSnippet => stack.Count > 0;

    public LoopState Current => stack.Peek();

    public ISet<string> GetIncludes() => usings;

    public void ExtractIncludes(string line, Func<string, string> includeExtractor)
    {
        var include = includeExtractor(line);
        if (include != null)
        {
            usings.Add(include);
        }
    }

    public void AppendLine(string line)
    {
        foreach (var state in stack)
        {
            state.AppendLine(line);
        }
    }

    public void Pop()
    {
        stack.Pop();
    }

    public void Push(Func<string, bool> endFunc, string key, int startLine, string version)
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

    HashSet<string> usings = new HashSet<string>();
    Stack<LoopState> stack = new Stack<LoopState>();
}