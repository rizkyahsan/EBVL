namespace EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

public static class GetUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetUser)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{userId}";
    }
}
