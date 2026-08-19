namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

public static class UpdateEmailTemplateRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateEmailTemplate)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Update} {EmailTemplatesDisplayTextFor.EmailTemplate}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{emailTemplateId:guid}}";

    public static string ResourceUri(Guid emailTemplateId)
    {
        return $"{RouteConfig.BasePath}/{emailTemplateId}";
    }
}
