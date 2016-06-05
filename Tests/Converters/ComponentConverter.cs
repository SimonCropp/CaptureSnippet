using System;
using CaptureSnippets;
using Newtonsoft.Json;

class ComponentConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var component = (Component)value;
        serializer.Serialize(writer, component.ValueOrUndefined);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Component);
    }
}