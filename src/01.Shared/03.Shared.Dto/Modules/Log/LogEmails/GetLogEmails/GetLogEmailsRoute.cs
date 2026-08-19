namespace EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;

public static class GetLogEmailsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLogEmails)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {LogEmailsDisplayTextFor.LogEmails}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
