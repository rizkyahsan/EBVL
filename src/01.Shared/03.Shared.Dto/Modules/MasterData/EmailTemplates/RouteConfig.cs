namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(EmailTemplates)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(EmailTemplates)}";
}
