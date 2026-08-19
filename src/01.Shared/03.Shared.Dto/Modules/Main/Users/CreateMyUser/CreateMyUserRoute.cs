namespace EBVL.Shared.Dto.Modules.Main.Users.CreateMyUser;

public static class CreateMyUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CreateMyUser)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Create} {UsersDisplayTextFor.MyUser}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}";
}
