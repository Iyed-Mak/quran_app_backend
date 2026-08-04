using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuranSchool.Api.Json;

/// <summary>
/// يوحّد تواريخ `DateTime` على UTC عند القراءة والكتابة.
///
/// عمود `timestamp with time zone` في PostgreSQL لا يقبل سوى تواريخ
/// UTC، بينما ترسل واجهة Flutter تواريخ محلية بدون مؤشر منطقة زمنية
/// (مثل `2026-08-01T21:30:00.123456`) فيتحوّل عند فكّها إلى
/// `Kind=Unspecified` ويفشل الحفظ. هذا المحوّل يعامل القيمة غير
/// المؤشِّرة على أنها UTC، ويحوّل أي قيمة محلية إلى UTC.
/// </summary>
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException("DateTime value is null.");
        }

        if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var dt))
        {
            throw new JsonException($"Unable to parse '{value}' as DateTime.");
        }

        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            _ => dt.ToUniversalTime(),
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(
            value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
}
