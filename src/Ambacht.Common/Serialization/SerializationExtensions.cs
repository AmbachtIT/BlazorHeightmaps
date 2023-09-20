using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambacht.Common.Serialization
{
    public static class SerializationExtensions
    {

        public static T Clone<T>(this IJsonSerializer serializer, T obj)
        {
            var json = serializer.SerializeObject(obj);
            return serializer.DeserializeObject<T>(json);
        }

        public static void ExpectPropertyName(this ref Utf8JsonReader reader, string propertyName)
        {
            reader.ExpectTokenType(JsonTokenType.PropertyName);
            var value = reader.GetString();
            if (value != propertyName)
            {
                throw new JsonException($"Expected json property name {propertyName}, got {value}");
            }
        }

        public static string ExpectString(this ref Utf8JsonReader reader)
        {
            reader.ExpectTokenType(JsonTokenType.String);
            return reader.GetString();
        }

        public static void ExpectTokenType(this ref Utf8JsonReader reader, JsonTokenType expected)
        {
            if (!reader.Read())
            {
                throw new JsonException($"Unexpected end of json contents while expecting {expected}");
            }
            if (reader.TokenType != expected)
            {
                throw new JsonException($"Expected {expected}, got {reader.TokenType}");
            }
        }

        public static T ExpectValue<T>(this ref Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException($"Unexpected end of json contents while expecting value of type {typeof(T)}");
            }
            if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"Expected string or number, got {reader.TokenType}");
            }

            return ValueTypeSerializers.GetSerializer<T>().GetValue(ref reader);
        }

        public static float ExpectFloat(this ref Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException($"Unexpected end of json contents while expecting value of type float");
            }
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"Expected number, got {reader.TokenType}");
            }

            return reader.GetSingle();
        }

    }
}
