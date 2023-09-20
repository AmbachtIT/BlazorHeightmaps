using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambacht.Common.Serialization
{
    public abstract class ValueTypeSerializer<T>
    {
        public abstract T GetValue(ref Utf8JsonReader reader);

        public abstract void WriteValue(Utf8JsonWriter writer, T value);

    }

    public class FloatValueTypeSerializer : ValueTypeSerializer<float>
    {
        public override float GetValue(ref Utf8JsonReader reader)
        {
            return reader.GetSingle();
        }

        public override void WriteValue(Utf8JsonWriter writer, float value)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class DoubleValueTypeSerializer : ValueTypeSerializer<double>
    {
        public override double GetValue(ref Utf8JsonReader reader)
        {
            return reader.GetDouble();
        }

        public override void WriteValue(Utf8JsonWriter writer, double value)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class DateTimeValueTypeSerializer : ValueTypeSerializer<DateTime>
    {
        public override DateTime GetValue(ref Utf8JsonReader reader)
        {
            return reader.GetDateTime();
        }

        public override void WriteValue(Utf8JsonWriter writer, DateTime value)
        {
            writer.WriteStringValue(value);
        }
    }

    public static class ValueTypeSerializers
    {
        public static ValueTypeSerializer<T> GetSerializer<T>()
        {
            if (typeof(T) == typeof(double))
            {
                return (ValueTypeSerializer<T>)(object)new DoubleValueTypeSerializer();
            }
            if (typeof(T) == typeof(float))
            {
                return (ValueTypeSerializer<T>)(object)new FloatValueTypeSerializer();
            }

            if (typeof(T) == typeof(DateTime))
            {
                return (ValueTypeSerializer<T>)(object)new DateTimeValueTypeSerializer();
            }

            throw new NotImplementedException();
        }
    }
}
