using System;
using CaptureSnippets;
using Newtonsoft.Json;
using NuGet.Versioning;

class VersionRangeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }
        
        var version = (VersionRange)value;
        writer.WriteValue(version.ToFriendlyString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(VersionRange);
    }
}