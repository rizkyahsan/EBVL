namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Statics;

public static class RouteFor
{
    public const string Index = nameof(MyProjects);

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }

    public static string StageDetails(Guid id)
    {
        return $"{Index}/{nameof(StageDetails)}/{id}";
    }

    public static string StageUpdate(Guid id)
    {
        return $"{Index}/{nameof(StageUpdate)}/{id}";
    }
}
