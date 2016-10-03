using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var converters = ObjectApprover.JsonSerializer.Converters;
        converters.Add(new VersionRangeConverter());
        converters.Add(new ProcessResultConverter());
        converters.Add(new SnippetConverter());
    }
}