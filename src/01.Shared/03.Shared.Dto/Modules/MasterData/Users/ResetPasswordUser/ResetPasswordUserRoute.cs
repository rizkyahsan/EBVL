namespace EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;

public static class ResetPasswordUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ResetPasswordUser)}";
    public static readonly string Description = $"{UsersDisplayTextFor.ResetPassword} {UsersDisplayTextFor.User}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}/ResetPassword";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{userId}/ResetPassword";
    }
}
