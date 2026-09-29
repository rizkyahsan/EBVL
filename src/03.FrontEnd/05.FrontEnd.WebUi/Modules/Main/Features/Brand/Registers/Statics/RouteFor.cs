namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Statics;

public static class RouteFor
{
    public const string Index = "Brand/Register";
    public const string Create = Index + "/Create";

    public static string Detail(Guid id)
    {
        return $"{Index}/{id}";
    }
}
