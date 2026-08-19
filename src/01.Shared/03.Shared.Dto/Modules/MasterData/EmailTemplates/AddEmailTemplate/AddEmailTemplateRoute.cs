namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.AddEmailTemplate;

public static class AddEmailTemplateRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddEmailTemplate)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Add} {EmailTemplatesDisplayTextFor.EmailTemplate}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
