namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Statics;

public static class RouteFor
{
    public const string Index = "Vendor/RequestRegistrations";

    public static string Details(Guid requestRegistrationId)
    {
        return $"{Index}/{requestRegistrationId}";
    }
}
