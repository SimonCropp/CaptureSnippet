using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var converters = ObjectApprover.JsonSerializer.Converters;
        converters.Add(new VersionRangeConverter());
        converters.Add(new CachedSnippetsConverter());
        converters.Add(new SnippetGroupConverter());
        converters.Add(new IncludeGroupConverter());
        converters.Add(new SnippetPackageGroupConverter());
        converters.Add(new PackageConverter());
        converters.Add(new ProcessResultConverter());
        converters.Add(new ComponentConverter());
        converters.Add(new ReadSnippetsConverter());
        converters.Add(new ReadSnippetConverter());
        converters.Add(new ReadIncludeConverter());
        converters.Add(new SnippetVersionGroupConverter());
    }
}