namespace CaptureSnippets
{
    /// <summary>
    /// Used to translate a package alias to a full package name.
    /// </summary>
    public delegate Result<string> TranslatePackage(string path, string packageAlias);
}