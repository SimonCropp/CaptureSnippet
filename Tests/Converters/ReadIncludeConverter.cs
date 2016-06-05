using System;
using CaptureSnippets;
using Newtonsoft.Json;

class ReadIncludeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var readInclude = (ReadInclude)value;
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        serializer.Serialize(writer, readInclude.Key);
        if (!readInclude.IsInError)
        {
            writer.WritePropertyName("Component");
            serializer.Serialize(writer, readInclude.Component.ValueOrUndefined);
            writer.WritePropertyName("Package");
            serializer.Serialize(writer, readInclude.Package.ValueOrUndefined);
            writer.WritePropertyName("Version");
            serializer.Serialize(writer, readInclude.Version);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, readInclude.Value);
        }
        writer.WritePropertyName("Error");
        serializer.Serialize(writer, readInclude.Error);
        writer.WritePropertyName("IsInError");
        serializer.Serialize(writer, readInclude.IsInError);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ReadInclude);
    }
}