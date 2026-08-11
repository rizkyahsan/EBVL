namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Audits.Statics;

public static class RouteFor
{
    public const string Index = $"{AdministrationRouteFor.Index}/{nameof(Audits)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
