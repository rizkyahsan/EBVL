namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmailWithTemplate;

public static class SendEmailWithTemplateRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendEmailWithTemplate)}";
    public const string Description = $"{CommonDisplayTextFor.Send} {CommonDisplayTextFor.Email}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(SendEmailWithTemplate)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(SendEmailWithTemplate)}";
}
