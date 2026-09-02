namespace EBVL.Shared.Dto.Modules.MasterData.Users.ForgotPasswordUser;

public static class ForgotPasswordUserRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ForgotPasswordUser)}";
    public const string Description = "Request a password reset";
    public const string Pattern = $"{RouteConfig.BasePath}/ForgotPasswordUser";
    public const string ResourceUri = Pattern;
}
