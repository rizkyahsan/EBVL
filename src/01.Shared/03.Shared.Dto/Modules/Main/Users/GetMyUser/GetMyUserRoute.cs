namespace EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

public static class GetMyUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetMyUser)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {UsersDisplayTextFor.MyUser}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
}
