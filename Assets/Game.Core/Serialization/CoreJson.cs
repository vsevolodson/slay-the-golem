using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Game.Core.Serialization
{
    public static class CoreJson
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter() },
            Formatting = Formatting.Indented
        };

        public static string Serialize<T>(T value) => JsonConvert.SerializeObject(value, Settings);
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);
    }
}