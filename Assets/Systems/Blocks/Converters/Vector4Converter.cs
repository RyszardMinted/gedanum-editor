using Newtonsoft.Json;
using UnityEngine;

public class Vector4Converter : JsonConverter<Vector4> {
    public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer) {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteValue(value.w);
        writer.WriteEndArray();
    }

    public override Vector4 ReadJson(JsonReader reader, System.Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        var array = serializer.Deserialize<float[]>(reader);
        if (array.Length != 4) {
            throw new JsonSerializationException($"Expected an array of 4 elements, but got {array.Length}.");
        }
        return new Vector4(array[0], array[1], array[2], array[3]);
    }
}