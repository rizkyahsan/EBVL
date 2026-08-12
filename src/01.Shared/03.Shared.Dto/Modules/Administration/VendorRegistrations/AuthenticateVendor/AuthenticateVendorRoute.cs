namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public static class AuthenticateVendorRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AuthenticateVendor)}";
    public const string Description = "Authenticate a vendor local account";
    public const string Pattern = $"{RouteConfig.BasePath}/Authenticate";
    public const string ResourceUri = Pattern;
}
