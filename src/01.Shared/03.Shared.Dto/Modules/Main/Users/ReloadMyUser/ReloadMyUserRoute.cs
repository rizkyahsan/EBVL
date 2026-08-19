namespace EBVL.Shared.Dto.Modules.Main.Users.ReloadMyUser;

public static class ReloadMyUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ReloadMyUser)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Reload} {UsersDisplayTextFor.MyUser}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Reload}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Reload}";
}
