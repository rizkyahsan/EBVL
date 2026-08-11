namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Statics;

public static class RouteFor
{
    public const string Index = $"{MasterDataRouteFor.Index}/{nameof(Countries)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
