namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendResetPasswordUser;

public static class SendResetPasswordUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendResetPasswordUser)}";
    public const string Description = $"{CommonDisplayTextFor.Send} Reset Password {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}ResetPassword{UsersDisplayTextFor.User}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}ResetPassword{UsersDisplayTextFor.User}/{userId}";
    }
}
