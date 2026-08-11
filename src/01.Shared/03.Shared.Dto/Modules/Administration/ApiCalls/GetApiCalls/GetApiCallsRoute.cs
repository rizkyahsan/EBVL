namespace EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

public static class GetApiCallsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetApiCalls)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ApiCallsDisplayTextFor.ApiCalls}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
