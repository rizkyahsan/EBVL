namespace EBVL.Shared.Dto.Modules.Main.Users.UpdateMyUser;

public static class UpdateMyUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateMyUser)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {UsersDisplayTextFor.MyUser}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
}
