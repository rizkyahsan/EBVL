namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Statics;

public static class RouteFor
{
    public const string Index = $"{MasterDataRouteFor.Index}/{nameof(Lenders)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
