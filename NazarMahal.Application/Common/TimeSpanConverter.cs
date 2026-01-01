using System.Text.Json;
using System.Text.Json.Serialization;

namespace NazarMahal.Application.Common
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return TimeSpan.Zero;
            }

            if (TimeSpan.TryParse(value, out var timeSpan))
            {
                return timeSpan;
            }

            // Try parsing as "HH:mm:ss" format
            var parts = value.Split(':');
            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out var hours) &&
                    int.TryParse(parts[1], out var minutes) &&
                    int.TryParse(parts[2], out var seconds))
                {
                    return new TimeSpan(hours, minutes, seconds);
                }
            }

            throw new JsonException($"Unable to convert '{value}' to TimeSpan");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
        }
    }
}

