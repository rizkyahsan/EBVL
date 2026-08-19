namespace EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

public static class CheckVerificationUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CheckVerificationUser)}";
    public const string Description = $"Check {CommonDisplayTextFor.Verification} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{userId:guid}}/Check{CommonDisplayTextFor.Verification}{UsersDisplayTextFor.User}";

    public static string ResourceUri(Guid userId, string token)
    {
        return $"{RouteConfig.BasePath}/{userId}/Check{CommonDisplayTextFor.Verification}{UsersDisplayTextFor.User}?token={Uri.EscapeDataString(token)}";
    }
}
