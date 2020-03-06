using Newtonsoft.Json;

namespace E2E.Load.Core
{
    public class JsonSerializer
    {
        public string Serialize<TEntity>(TEntity entityToBeSerialized) => JsonConvert.SerializeObject(
            entityToBeSerialized,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            });

        public TEntity Deserialize<TEntity>(string content) => JsonConvert.DeserializeObject<TEntity>(content,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            });
    }
}