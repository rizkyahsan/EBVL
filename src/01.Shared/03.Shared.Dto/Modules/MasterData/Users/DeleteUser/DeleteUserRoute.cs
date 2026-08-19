namespace EBVL.Shared.Dto.Modules.MasterData.Users.DeleteUser;

public static class DeleteUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteUser)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{userId}";
    }
}
