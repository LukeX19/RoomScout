using Newtonsoft.Json;
using System.Globalization;

namespace RoomScout.DataAccess.Converters
{
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyyMMdd";

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var raw = reader.Value?.ToString();
            return DateTime.ParseExact(raw!, Format, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Format));
        }
    }
}
