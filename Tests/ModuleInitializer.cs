using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            var converters = settings.Converters;
            converters.Add(new VersionRangeConverter());
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        };
    }
}