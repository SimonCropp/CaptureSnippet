using System;
using CaptureSnippets;
using Newtonsoft.Json;

class ReadSnippetsConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var readSnippets = (ReadSnippets)value;
        writer.WriteStartObject();
        writer.WritePropertyName("snippets");
        serializer.Serialize(writer, readSnippets.Snippets);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ReadSnippets);
    }
}