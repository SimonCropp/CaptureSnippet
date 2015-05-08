using System;
using Newtonsoft.Json;
using Version = CaptureSnippets.Version;

class SnippetVersionConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }
        var version = (Version)value;
        writer.WriteValue(version.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (Version);
    }
}