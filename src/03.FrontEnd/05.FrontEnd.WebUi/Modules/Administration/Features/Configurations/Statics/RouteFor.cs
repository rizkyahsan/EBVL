namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Statics;

public static class RouteFor
{
    public const string Index = $"{AdministrationRouteFor.Index}/{nameof(Configurations)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
