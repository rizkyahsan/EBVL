namespace EBVL.FrontEnd.WebUi.Modules.Log.Features.LogEmails.Statics;

public static class RouteFor
{
    public const string Index = $"{LogRouteFor.Index}/{nameof(LogEmails)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
