using System;
using Newtonsoft.Json;

namespace Game.Core.State
{
    internal sealed class CardIdJsonConverter : JsonConverter<CardId>
    {
        public override void WriteJson(JsonWriter writer, CardId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Value);
        }

        public override CardId ReadJson(JsonReader reader, Type objectType, CardId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            return new CardId((string)reader.Value);
        }
    }
}
