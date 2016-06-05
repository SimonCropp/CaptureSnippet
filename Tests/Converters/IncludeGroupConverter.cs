using System;
using CaptureSnippets;
using Newtonsoft.Json;

class IncludeGroupConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var includeGroup = (IncludeGroup)value;
        writer.WriteStartObject();
        writer.WritePropertyName("key");
        writer.WriteValue(includeGroup.Key);
        writer.WritePropertyName("component");
        writer.WriteValue(includeGroup.Component.ValueOrUndefined);
        writer.WritePropertyName("packages");
        serializer.Serialize(writer, includeGroup.Packages);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(IncludeGroup);
    }
}