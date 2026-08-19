namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplates;

public static class GetEmailTemplatesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetEmailTemplates)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {EmailTemplatesDisplayTextFor.EmailTemplates}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
