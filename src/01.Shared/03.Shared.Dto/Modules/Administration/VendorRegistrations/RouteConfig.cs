namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(VendorRegistrations)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(VendorRegistrations)}";
}
