namespace EBVL.Shared.Statics.Common.Extensions;

public static class DateTimeExtensions
{
    public static string ToShortDateDisplayText(this DateTime dateTime)
    {
        return dateTime.ToString("dd-MM-yyyy");
    }

    public static string ToLongDateTimeDisplayText(this DateTime dateTime)
    {
        return dateTime.ToString("dd MMMM yyyy HH:mm:ss");
    }

    public static string ToCompleteDateTimeDisplayText(this DateTime dateTime)
    {
        return dateTime.ToString("dd MMMM yyyy HH:mm:ss \"UTC\"zzz");
    }
}
