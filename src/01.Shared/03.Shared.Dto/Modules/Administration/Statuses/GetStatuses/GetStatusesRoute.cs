namespace EBVL.Shared.Dto.Modules.Administration.Statuses.GetStatuses;

public static class GetStatusesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetStatuses)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {StatusesDisplayTextFor.Statuses}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
