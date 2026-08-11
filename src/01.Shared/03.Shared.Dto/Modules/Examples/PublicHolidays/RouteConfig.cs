namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(PublicHolidays)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(PublicHolidays)}";
}
