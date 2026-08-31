namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorMonitoring.RegistrationVendors.Statics;

public static class RouteFor
{
    public const string Index = "VendorMonitoring/RegistrationVendors";

    public static string Details(Guid id)
    {
        return $"{Index}/{id}";
    }
}
