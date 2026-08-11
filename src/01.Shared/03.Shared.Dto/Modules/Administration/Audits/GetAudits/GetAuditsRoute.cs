namespace EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

public static class GetAuditsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetAudits)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {AuditsDisplayTextFor.Audits}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
