namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

public static class DeleteEmailTemplateRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteEmailTemplate)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Delete} {EmailTemplatesDisplayTextFor.EmailTemplate}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{emailTemplateId:guid}}";

    public static string ResourceUri(Guid emailTemplateId)
    {
        return $"{RouteConfig.BasePath}/{emailTemplateId}";
    }
}
