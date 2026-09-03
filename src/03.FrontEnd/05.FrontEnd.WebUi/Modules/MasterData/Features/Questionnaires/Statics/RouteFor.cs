namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Statics;

public static class RouteFor
{
    public const string Index = MasterDataRouteFor.Index + "/Questionnaires";
    public static string Details(Guid id)
    {
        return $"{Index}/Details/{id}";
    }

    public static string Preview(Guid id)
    {
        return $"{Index}/Preview/{id}";
    }
}
