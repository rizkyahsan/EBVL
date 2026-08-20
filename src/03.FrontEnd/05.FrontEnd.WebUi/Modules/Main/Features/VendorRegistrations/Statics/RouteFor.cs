namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics;

public static class RouteFor
{
    public const string Index = "Vendor-Registration";
    public const string Sap = $"{Index}/SAP";
    public const string StepOne = $"{Index}/Step-1";

    public static string StepOneWith(string sapVendorNumber)
    {
        return $"{StepOne}?sapVendorNumber={Uri.EscapeDataString(sapVendorNumber)}";
    }
}
