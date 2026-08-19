namespace EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;

public static class AddUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddUser)}";
    public const string Description = $"{CommonDisplayTextFor.Add} {UsersDisplayTextFor.User}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
