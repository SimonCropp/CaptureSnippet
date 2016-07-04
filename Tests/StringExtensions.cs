public static class StringExtensions
{
    public static string FixNewLines(this string target)
    {
        return target.Replace("\r\n", "\n");
    }
    public static string TrimTrailingNewLine(this string target)
    {
        return target.TrimEnd('\r', '\n');
    }
}