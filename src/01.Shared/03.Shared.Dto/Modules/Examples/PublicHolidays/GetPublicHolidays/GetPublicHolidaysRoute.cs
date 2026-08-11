namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

public static class GetPublicHolidaysRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetPublicHolidays)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {PublicHolidaysDisplayTextFor.PublicHolidays}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
