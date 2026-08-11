namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays.LoadPublicHolidays;

public static class LoadPublicHolidaysRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(LoadPublicHolidays)}";
    public const string Description = $"{CommonDisplayTextFor.Load} {PublicHolidaysDisplayTextFor.PublicHolidays}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
