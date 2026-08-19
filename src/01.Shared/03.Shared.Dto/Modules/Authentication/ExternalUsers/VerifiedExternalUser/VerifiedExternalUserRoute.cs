namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public static class VerifiedExternalUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(VerifiedExternalUser)}";
    public const string Description = $"{CommonDisplayTextFor.Verified} External {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(VerifiedExternalUser)}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{nameof(VerifiedExternalUser)}/{id}";
    }
}
