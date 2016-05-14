namespace CaptureSnippets
{
    public delegate Result<SnippetMetaData> ExtractMetaDataFromPath(string rootDirectory, string fileOrDirectoryPath, SnippetMetaData parent);
}