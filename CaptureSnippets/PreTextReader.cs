using System.IO;

static class PreTextReader
{
    public static string GetPretext(string directoryPath)
    {
        string pretext = null;
        var preReleaseFilePath = Path.Combine(directoryPath, "prerelease.txt");
        if (File.Exists(preReleaseFilePath))
        {
            pretext = "pre";
            var fileContents = File.ReadAllText(preReleaseFilePath).Trim();
            if (!string.IsNullOrWhiteSpace(fileContents))
            {
                pretext = fileContents;
            }
        }
        return pretext;
    }
}