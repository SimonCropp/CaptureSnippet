using System;
using CaptureSnippets;
using Newtonsoft.Json;

class CachedSnippetsConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var cachedSnippets = (CachedSnippets)value;
        writer.WriteStartObject();
        writer.WritePropertyName("ticks");
        writer.WriteValue(cachedSnippets.Ticks);
        writer.WritePropertyName("snippetGroups");
        serializer.Serialize(writer, cachedSnippets.SnippetGroups);
        writer.WritePropertyName("errors");
        serializer.Serialize(writer, cachedSnippets.ReadingErrors);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(CachedSnippets);
    }
}