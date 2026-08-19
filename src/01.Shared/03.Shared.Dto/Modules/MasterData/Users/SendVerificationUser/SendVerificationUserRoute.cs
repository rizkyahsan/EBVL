namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendVerificationUser;

public static class SendVerificationUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendVerificationUser)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {CommonDisplayTextFor.Verification} {UsersDisplayTextFor.User}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}{CommonDisplayTextFor.Verification}{UsersDisplayTextFor.User}/{{userId:guid}}";

    public static string ResourceUri(Guid userId)
    {
        return $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Send}{CommonDisplayTextFor.Verification}{UsersDisplayTextFor.User}/{userId}";
    }
}
