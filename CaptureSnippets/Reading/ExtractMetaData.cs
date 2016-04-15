namespace CaptureSnippets
{
    public delegate Result<SnippetMetaData> ExtractMetaData(string fileOrDirectoryPath, SnippetMetaData parent);
}