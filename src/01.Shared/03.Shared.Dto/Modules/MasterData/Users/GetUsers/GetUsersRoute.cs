namespace EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

public static class GetUsersRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetUsers)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {UsersDisplayTextFor.Users}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
