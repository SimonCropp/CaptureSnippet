using Newtonsoft.Json;
using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var converters = ObjectApprover.JsonSerializer.Converters;
        converters.Add(new VersionRangeConverter());
        converters.Add(new CachedSnippetsConverter());
        converters.Add(new SnippetGroupConverter());
        converters.Add(new ProcessResultConverter());
        converters.Add(new ReadSnippetsConverter());
        converters.Add(new VersionGroupConverter());
    }
}