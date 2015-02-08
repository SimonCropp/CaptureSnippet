using System;
using System.Collections.Generic;
using System.IO;

namespace CaptureSnippets
{
    public static class DirectoryDateFinder
    {

        public static long GetLastDirectoryWrite(IEnumerable<string> directories)
        {
            var lastHigh = DateTime.MinValue;
            foreach (var directory in directories)
            {
                var lastWriteTime = File.GetLastWriteTime(directory);
                if (lastWriteTime > lastHigh)
                {
                    lastHigh = lastWriteTime;
                }
            }
            return lastHigh.Ticks;
        }
    }
}