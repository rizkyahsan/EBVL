namespace EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUser;

public static class UpdateUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateUser)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{userId}";
    }
}
