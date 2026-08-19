namespace EBVL.Shared.Dto.Modules.Main.Users.VerifyMyUser;

public static class VerifyMyUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(VerifyMyUser)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Verify} {UsersDisplayTextFor.MyUser}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Verify}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Verify}";
}
