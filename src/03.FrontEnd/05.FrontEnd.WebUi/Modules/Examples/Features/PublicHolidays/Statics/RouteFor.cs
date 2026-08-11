namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.PublicHolidays.Statics;

public static class RouteFor
{
    public const string Index = $"{ExamplesRouteFor.Index}/{nameof(PublicHolidays)}";
    public const string ApiCallsHistory = $"{Index}/{nameof(ApiCallsHistory)}";
}
