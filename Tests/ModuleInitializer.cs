using Newtonsoft.Json;
using ObjectApproval;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var versionConverter = new VersionRangeConverter();
        ObjectApprover.JsonSerializer.Converters.Add(versionConverter);
    }
}