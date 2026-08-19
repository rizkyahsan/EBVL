namespace EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

public static class GetLogEmailRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLogEmail)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {LogEmailsDisplayTextFor.LogEmail}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
