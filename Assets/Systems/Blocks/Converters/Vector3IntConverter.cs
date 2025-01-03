using Newtonsoft.Json;
using UnityEngine;

public class Vector3IntConverter : JsonConverter<Vector3Int> {
    public override void WriteJson(JsonWriter writer, Vector3Int value, JsonSerializer serializer) {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteEndArray();
    }

    public override Vector3Int ReadJson(JsonReader reader, System.Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer) {
        var array = serializer.Deserialize<int[]>(reader);
        if (array.Length != 3) {
            throw new JsonSerializationException($"Expected an array of 3 elements, but got {array.Length}.");
        }
        return new Vector3Int(array[0], array[1], array[2]);
    }
}