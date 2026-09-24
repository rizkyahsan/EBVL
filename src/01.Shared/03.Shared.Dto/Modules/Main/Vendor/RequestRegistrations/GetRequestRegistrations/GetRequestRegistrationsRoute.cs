namespace EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public static class GetRequestRegistrationsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetRequestRegistrations)}";
    public const string Description = "Get vendor registration requests";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
