namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Statics;

public static class RouteFor
{
    public const string Index = MasterDataRouteFor.Index + "/Questionnaires";
    public static string Details(Guid id, Guid sectionId)
    {
        return $"{Index}/Details/{id}/{sectionId}";
    }

    public static string Details(Guid id)
    {
        return $"{Index}/Details/{id}";
    }

    public static string Preview(Guid id)
    {
        return $"{Index}/Preview/{id}";
    }

    public static string History(Guid seriesId)
    {
        return $"{Index}/History/{seriesId}";
    }
}
