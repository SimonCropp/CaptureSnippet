using System;
using CaptureSnippets;
using Newtonsoft.Json;

class PackageConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var package = (Package)value;
        serializer.Serialize(writer, package.ValueOrUndefined);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Package);
    }
}