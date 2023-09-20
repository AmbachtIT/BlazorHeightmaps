using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambacht.Common.Serialization
{
    public class IdentifierJsonConverter<T> : JsonConverter<Identifier<T>>
    {
        public override Identifier<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var id = reader.GetString();
            return new Identifier<T>(id);
        }

        public override void Write(Utf8JsonWriter writer, Identifier<T> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }
}
