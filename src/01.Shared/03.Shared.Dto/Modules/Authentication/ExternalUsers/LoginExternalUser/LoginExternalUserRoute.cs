namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

public static class LoginExternalUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(LoginExternalUser)}";
    public const string Description = $"{CommonDisplayTextFor.Login} External User";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(LoginExternalUser)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(LoginExternalUser)}";
}
