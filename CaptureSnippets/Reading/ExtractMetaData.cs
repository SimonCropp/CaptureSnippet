namespace CaptureSnippets
{
    public delegate Result<SnippetMetaData> ExtractMetaData(string rootDirectory, string fileOrDirectoryPath, SnippetMetaData parent);
}