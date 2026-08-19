namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;

public static class CheckExternalUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CheckExternalUser)}";
    public const string Description = $"Check External {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(CheckExternalUser)}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{nameof(CheckExternalUser)}/{id}";
    }
}
