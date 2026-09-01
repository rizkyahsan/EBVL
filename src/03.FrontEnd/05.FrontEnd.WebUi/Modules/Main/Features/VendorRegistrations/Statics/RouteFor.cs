namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics;

public static class RouteFor
{
    public const string Index = "Vendor-Registration";
    public const string StepOne = $"{Index}/Step-1";
    public const string StepTwo = $"{Index}/Step-2";
    public const string StepThree = $"{Index}/Step-3";
    public const string Review = $"{Index}/Review";

    public static string StepOneWith(string sapVendorNumber)
    {
        return $"{StepOne}?sapVendorNumber={Uri.EscapeDataString(sapVendorNumber)}";
    }
}
