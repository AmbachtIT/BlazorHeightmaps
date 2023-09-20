using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambacht.Common.Serialization
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        public SystemTextJsonSerializer() : this(SystemTextJsonDefaults.CreateDefault(false)) {
        }

        public SystemTextJsonSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        private readonly JsonSerializerOptions _options;

        public string SerializeObject(object obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
