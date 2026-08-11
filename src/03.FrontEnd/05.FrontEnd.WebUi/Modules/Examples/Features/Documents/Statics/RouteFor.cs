namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Statics;

public static class RouteFor
{
    public const string Index = $"{ExamplesRouteFor.Index}/{nameof(Documents)}";

    public static string Details(Guid id)
    {
        return $"{Index}/{nameof(Details)}/{id}";
    }
}
