namespace EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;

public static class VerifiedUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(VerifiedUser)}";
    public const string Description = $"{CommonDisplayTextFor.Verified} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}/{CommonDisplayTextFor.Verified}{UsersDisplayTextFor.User}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{userId}/{CommonDisplayTextFor.Verified}{UsersDisplayTextFor.User}";
    }
}
