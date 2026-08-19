namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

public static class GetEmailTemplateRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetEmailTemplate)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {EmailTemplatesDisplayTextFor.EmailTemplate}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{emailTemplateId:guid}}";

    public static string ResourceUri(Guid emailTemplateId)
    {
        return $"{RouteConfig.BasePath}/{emailTemplateId}";
    }
}
