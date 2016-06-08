using System;
using CaptureSnippets;
using Newtonsoft.Json;

class SnippetVersionGroupConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var versionGroup = (VersionGroup)value;
        writer.WriteStartObject();
        writer.WritePropertyName("version");
        writer.WriteValue(versionGroup.Version.PrettyPrint());
        writer.WritePropertyName("value");
        writer.WriteValue(versionGroup.Value);
        writer.WritePropertyName("sources");
        serializer.Serialize(writer, versionGroup.Sources);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(VersionGroup);
    }
}