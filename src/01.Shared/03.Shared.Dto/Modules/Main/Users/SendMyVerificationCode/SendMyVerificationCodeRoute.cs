namespace EBVL.Shared.Dto.Modules.Main.Users.SendMyVerificationCode;

public static class SendMyVerificationCodeRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendMyVerificationCode)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {CommonDisplayTextFor.My} {CommonDisplayTextFor.VerificationCode}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Send}{CommonDisplayTextFor.VerificationCode}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.My}/{CommonDisplayTextFor.Send}{CommonDisplayTextFor.VerificationCode}";
}
