namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public static class SendOtpExternalUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendOtpExternalUser)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {UsersDisplayTextFor.Otp} External {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(SendOtpExternalUser)}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{nameof(SendOtpExternalUser)}/{id}";
    }
}

