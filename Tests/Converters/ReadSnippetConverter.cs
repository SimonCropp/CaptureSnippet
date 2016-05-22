using System;
using CaptureSnippets;
using Newtonsoft.Json;

class ReadSnippetConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var readSnippet = (ReadSnippet)value;
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        serializer.Serialize(writer, readSnippet.Key);
        if (!readSnippet.IsInError)
        {
            writer.WritePropertyName("Language");
            serializer.Serialize(writer, readSnippet.Language);
            writer.WritePropertyName("Package");
            serializer.Serialize(writer, readSnippet.Package.ValueOrNone);
            writer.WritePropertyName("Version");
            serializer.Serialize(writer, readSnippet.Version);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, readSnippet.Value);
        }
        writer.WritePropertyName("Error");
        serializer.Serialize(writer, readSnippet.Error);
        writer.WritePropertyName("FileLocation");
        serializer.Serialize(writer, readSnippet.FileLocation);
        writer.WritePropertyName("IsInError");
        serializer.Serialize(writer, readSnippet.IsInError);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ReadSnippet);
    }
}