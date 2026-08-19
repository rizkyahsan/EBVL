namespace EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUserPic;

public static class UpdateUserPicRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateUserPic)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Update} {UsersDisplayTextFor.UserPic}";
    public const string Pattern = $"{RouteConfig.BasePath}/UserPic/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/UserPic/{userId}";
    }
}
