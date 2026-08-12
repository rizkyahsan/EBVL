namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

public static class RegisterVendorRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(RegisterVendor)}";
    public const string Description = "Register a vendor local account and submit vendor evidence metadata";
    public const string Pattern = $"{RouteConfig.BasePath}/Register";
    public const string ResourceUri = Pattern;
}
