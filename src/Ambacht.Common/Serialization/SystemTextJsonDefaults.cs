using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Ambacht.Common.Serialization
{
    public static class SystemTextJsonDefaults
    {

        public static JsonSerializerOptions CreateDefault(bool indented) => new JsonSerializerOptions()
        {
            
        }.ConfigureDefaults(indented);


        public static JsonSerializerOptions ConfigureDefaults(this JsonSerializerOptions options, bool indented)
        {
            options.AllowTrailingCommas = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector3Converter());
            options.WriteIndented = indented;
            options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals |
                                     JsonNumberHandling.AllowReadingFromString;
            return options;
        }

    }
}
