namespace EBVL.Shared.Dto.Modules.Administration.Statuses.GetStatus;

public static class GetStatusRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetStatus)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {StatusesDisplayTextFor.Status}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{statusId:guid}}";

    public static string ResourceUri(Guid statusId)
    {
        return $"{RouteConfig.BasePath}/{statusId}";
    }
}
