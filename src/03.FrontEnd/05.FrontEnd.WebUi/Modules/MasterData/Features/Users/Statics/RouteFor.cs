namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Statics;

public static class RouteFor
{
    public const string Index = $"{MasterDataRouteFor.Index}/{nameof(Users)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
