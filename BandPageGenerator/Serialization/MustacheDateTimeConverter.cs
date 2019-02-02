using BandPageGenerator.Models;
using Newtonsoft.Json;
using System;

namespace BandPageGenerator.Serialization
{
    public class MustacheDateTimeConverter : JsonConverter<MustacheDateTime>
    {
        public override MustacheDateTime ReadJson(JsonReader reader, Type objectType, MustacheDateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new MustacheDateTime { DateTime = (DateTime)reader.Value };
        }

        public override void WriteJson(JsonWriter writer, MustacheDateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
