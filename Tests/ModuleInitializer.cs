using Newtonsoft.Json;
using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var versionConverter = new SnippetVersionConverter();
        ObjectApprover.JsonSerializer.Converters.Add(versionConverter);
    }
}