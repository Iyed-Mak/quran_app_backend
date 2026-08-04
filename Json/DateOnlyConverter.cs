using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuranSchool.Api.Json;

/// <summary>
/// يقبل تواريخ `DateOnly` بصيغة `yyyy-MM-dd` أو صيغة ISO كاملة (مع الوقت)
/// التي ترسلها واجهة Flutter، ويُخرجها بصيغة `yyyy-MM-dd`.
/// </summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (DateOnly.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, out date))
        {
            return date;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new JsonException($"Unable to parse '{value}' as DateOnly.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}
