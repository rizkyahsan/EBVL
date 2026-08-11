namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

public static class SendEmailRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendEmail)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {CommonDisplayTextFor.Email}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(SendEmail)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(SendEmail)}";
}
