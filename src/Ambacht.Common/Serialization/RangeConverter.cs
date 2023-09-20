using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Serialization
{
    public class RangeConverter<T> : JsonConverter<Range<T>> where T: IComparable<T>
    {

        private readonly ValueTypeSerializer<T> _valueTypeSerializer = ValueTypeSerializers.GetSerializer<T>();


        public override Range<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //reader.ExpectTokenType(JsonTokenType.StartArray);
            var min = reader.ExpectValue<T>();
            var max = reader.ExpectValue<T>();
            reader.ExpectTokenType(JsonTokenType.EndArray);
            return new Range<T>(min, max);
        }

        public override void Write(Utf8JsonWriter writer, Range<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            _valueTypeSerializer.WriteValue(writer, value.Min);
            _valueTypeSerializer.WriteValue(writer, value.Max);
            writer.WriteEndArray();
        }


    }
}
