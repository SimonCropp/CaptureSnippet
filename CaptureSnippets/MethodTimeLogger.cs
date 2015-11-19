using System.Reflection;

namespace CaptureSnippets
{
    static class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, long milliseconds)
        {
#if DEBUG
           System.Diagnostics.Trace.WriteLine($"{methodBase.Name} took {milliseconds}ms");
#endif
        }
    }
}