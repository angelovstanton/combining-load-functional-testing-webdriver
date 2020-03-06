using System.IO;
using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace E2E.Load.Core
{
    // Code from: https://www.bytefish.de/blog/restsharp_custom_json_serializer/
    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public string ContentType
        {
            get => "application/json";
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter);
            _serializer.Serialize(jsonTextWriter, obj);

            return stringWriter.ToString();
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using var stringReader = new StringReader(content);
            using var jsonTextReader = new JsonTextReader(stringReader);
            return _serializer.Deserialize<T>(jsonTextReader);
        }

        public static NewtonsoftJsonSerializer Default
        {
            get
            {
                return new NewtonsoftJsonSerializer(new Newtonsoft.Json.JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
        }
    }
}