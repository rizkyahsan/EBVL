namespace EBVL.Shared.Statics.Common.Extensions;

public static class DateTimeOffsetExtensions
{
    private static DateTime ConvertToZone(object date, TimeZoneInfo? timeZone = null)
    {
        DateTime dt;

        if (date is DateTimeOffset dto)
        {
            dt = dto.DateTime;
        }
        else if (date is DateTime d)
        {
            dt = d.Kind == DateTimeKind.Utc ? d : d.ToUniversalTime();
        }
        else
        {
            throw new ArgumentException("Unsupported type. Use DateTime or DateTimeOffset.");
        }

        return timeZone == null
            ? TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local)
            : TimeZoneInfo.ConvertTimeFromUtc(dt, timeZone);
    }

    public static string ToShortDateDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        return ConvertToZone(date, timeZone).ToString("dd-MM-yyyy");
    }

    public static string ToShortDateTimeDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        return ConvertToZone(date, timeZone).ToString("dd-MMM-yyyy HH:mm:ss");
    }

    public static string ToLongDateDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        return ConvertToZone(date, timeZone).ToString("dd MMMM yyyy");
    }

    public static string ToLongDateTimeDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        return ConvertToZone(date, timeZone).ToString("dd MMMM yyyy HH:mm:ss");
    }

    public static string ToCompleteDateTimeDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        return ConvertToZone(date, timeZone).ToString("dd MMMM yyyy HH:mm:ss \"UTC\"zzz");
    }

    public static string ToFriendlyTimeDisplayText(this object date, TimeZoneInfo? timeZone = null)
    {
        var local = ConvertToZone(date, timeZone);
        var hour = local.Hour;

        return hour is >= 4 and < 12
            ? "Morning"
            : hour is >= 12 and < 17
                ? "Afternoon"
                : hour is >= 17 and < 21
                    ? "Evening"
                    : "Night";
    }
}
