namespace EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

public static class GetAuditRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetAudit)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {AuditsDisplayTextFor.Audit}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{auditId:guid}}";

    public static string ResourceUri(Guid auditId)
    {
        return $"{RouteConfig.BasePath}/{auditId}";
    }
}
