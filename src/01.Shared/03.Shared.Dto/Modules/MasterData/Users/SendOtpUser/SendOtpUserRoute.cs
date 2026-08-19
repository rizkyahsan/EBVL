namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendOtpUser;

public static class SendOtpUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendOtpUser)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {UsersDisplayTextFor.Otp} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}{UsersDisplayTextFor.Otp}{UsersDisplayTextFor.User}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}{UsersDisplayTextFor.Otp}{UsersDisplayTextFor.User}/{userId}";
    }
}

