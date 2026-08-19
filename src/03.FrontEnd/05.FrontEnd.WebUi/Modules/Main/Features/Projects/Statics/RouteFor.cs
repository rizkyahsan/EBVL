namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Statics;

public static class RouteFor
{
    public const string Index = nameof(Projects);

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }

    public static string StageDetails(Guid id)
    {
        return $"{Index}/{nameof(StageDetails)}/{id}";
    }

    public static string Create()
    {
        return $"{Index}/{nameof(Create)}";
    }

    public static string StageCreate(Guid id)
    {
        return $"{Index}/{nameof(StageCreate)}/{id}";
    }

    public static string Update(Guid id)
    {
        return $"{Index}/{nameof(Update)}/{id}";
    }

    public static string StageUpdate(Guid id)
    {
        return $"{Index}/{nameof(StageUpdate)}/{id}";
    }

    public static string Verify()
    {
        return $"{Index}/{nameof(Verify)}";
    }

    public static string StageVerify(Guid id)
    {
        return $"{Index}/{nameof(StageVerify)}/{id}";
    }

    public static string Complete()
    {
        return $"{Index}/{nameof(Complete)}";
    }
}
