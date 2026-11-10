using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromVesClient.DTO
{
    // Результат взвешивания вагона, который приходит от сервера.
    public class RailcarWeightResultDto
    {
        // Вес первой боковой стороны (COM-порт 1). 0, если порт не используется.
        public decimal Side1Weight { get; set; }

        // Вес второй боковой стороны (COM-порт 2). 0, если порт не используется.
        public decimal Side2Weight { get; set; }

        // Вес третьей боковой стороны (COM-порт 3). 0, если порт не используется.
        public decimal Side3Weight { get; set; }

        // Вес четвёртой боковой стороны (COM-порт 4). 0, если порт не используется.
        public decimal Side4Weight { get; set; }

        // Общий вес вагона: сумма всех четырёх боковых сторон.
        public decimal TotalWeight { get; set; }

        // Вес, полученный с каждого COM-порта.
        public Dictionary<int, decimal> PortWeights { get; set; } = new();

        // Время начала и окончания прохода вагона.
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    // Принимает даты как в ISO 8601 (стандарт System.Text.Json),
    // так и в формате "dd.MM.yyyy HH:mm:ss", который использовался раньше.
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] SupportedFormats =
        {
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy HH:mm",
            "dd.MM.yyyy",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm"
        };

        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException("Пустое значение даты.");
            }

            if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime date))
            {
                return date;
            }

            if (DateTime.TryParseExact(
                    value,
                    SupportedFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                return date;
            }

            throw new JsonException($"Не удалось разобрать дату: {value}");
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
